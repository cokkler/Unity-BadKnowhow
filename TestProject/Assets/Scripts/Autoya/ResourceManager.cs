using System;
using System.IO;
using System.Collections;
using UnityEngine;
using AutoyaFramework;
using AutoyaFramework.AssetBundles;
using UniRx;

#if UNITY_EDITOR && !DOWNLOAD_ASSETBUNDLES
using UnityEditor;
#endif

public class ResourceManager : MonoBehaviour
{
	public bool IsAutoyaReady { get; set; }
	
	private void Start() {
		StartCoroutine(initializeAutoya());
	}
	
	/// <summary>
	/// Autoya初期化
	/// </summary>
	/// <returns></returns>
	private IEnumerator initializeAutoya() {
		//認証待ち(Autoyaのインスタンス生成も兼ねてるようなので必須)
		while (!Autoya.Auth_IsAuthenticated()) {
			yield return null;
		}

#if !UNITY_EDITOR || DOWNLOAD_ASSETBUNDLES
		//AssetBundleListのロード(ダウンロード)
		Autoya.AssetBundle_DownloadAssetBundleListsIfNeed(
			status => { Debug.LogFormat("status:{0}", status); },
			(code, reason, autoyaStatus) => { Debug.LogFormat("code:{0} reason:{1}, autoyaStatus:{2}", code, reason, autoyaStatus); }
		);
		// wait downloading assetBundleList.
		while (!Autoya.AssetBundle_IsAssetBundleFeatureReady()) {
			yield return null;
		}
#endif
		
		Debug.Log("[ResourceManager] Autoya is Ready!!");
		IsAutoyaReady = true;
	}
	
	/// <summary>
	/// Assetデータのロード
	/// EDITOR時はAssetDatabaseを使って直接Assetを参照し返す
	/// 実機の場合はAutoya経由で該当のリソースが入ったAssetBundleをロードorダウンロードする
	/// 
	/// どちらの場合もAssets以下のPATHで読み込めるので、その部分で利用側がEDITOR、実機を意識する必要がない。
	/// 
	/// </summary>
	/// <returns>The asset bundle async.</returns>
	/// <param name="path">Path.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public IObservable<T> LoadAssetBundleAsync<T>(string path) where T : UnityEngine.Object =>
		Observable.Create<T>(observer => {
#if UNITY_EDITOR && !DOWNLOAD_ASSETBUNDLES
			var obj = AssetDatabase.LoadAssetAtPath<T>(path);
			if (obj) {
				observer.OnNext(obj);
				observer.OnCompleted();
			}
#else
			Observable.EveryUpdate()
				.Where(_ => IsAutoyaReady)
				.Take(1)
				.Subscribe(_ => {
					autoyaLoadAsset(path, observer);
				});
#endif
			return Disposable.Empty;
		});

	private void autoyaLoadAsset<T>(string path, IObserver<T> observer) where T : UnityEngine.Object {
		//GetContainedAssetBundleName
		Autoya.AssetBundle_LoadAsset<T>
		(
			assetName: path,
			loadSucceeded: (name, obj) => {
				observer.OnNext(obj);
				observer.OnCompleted();
			},
			loadFailed: (name, err, reason, status) => {
				var message = string.Format(
					"BINARY autoyaLoadAsset error name:{0} err:{1} reason:{2} status:{3}",
					name, err, reason, status
				);
				Exception ex;
				if (err == AssetBundleLoadError.NotContainedAssetBundle) {
					ex = new FileNotFoundException(message, path);
				}
				else {
					ex = new FileLoadException(message, path);
				}
				observer.OnError(ex);
			}
		);
	}
}
