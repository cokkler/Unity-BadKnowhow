using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

public class UniRxScene : MonoBehaviour
{
	[SerializeField]
	private GameObject baseSphere = null;
	[SerializeField]
	private Button makeSphereButton = null;
	[SerializeField]
	private Button reloadButton = null;
	[SerializeField]
	private Image fadeImage = null;

	/// <summary>
	/// スクリーンクリックするたびクリックされたスクリーン座標のメッセージを発行する コルーチン側
	/// </summary>
	/// <returns>The click coroutine.</returns>
	/// <param name="subject">Subject.</param>
	private IEnumerator ScreenClickCoroutine(Subject<Vector3> subject) {
		while (true) {
			if (Input.GetMouseButton(0)) {
				subject.OnNext(Input.mousePosition);
			}
			yield return null;
		}
	}

	/// <summary>
	/// スクリーンクリックするたびクリックされたスクリーン座標のメッセージを発行する
	/// </summary>
	/// <returns>The click as observable.</returns>
	private IObservable<Vector3> ScreenClickAsObservable() {
		var subject = new Subject<Vector3>();	//メッセージの発行元 OnNextでメッセージ発行、OnCompletedで発行終了
		StartCoroutine(ScreenClickCoroutine(subject));
		return subject;
	}
	
	/// <summary>
	/// repeatCount回空のメッセージを発行する
	/// </summary>
	/// <returns>The repeater.</returns>
	/// <param name="repeatCount">メッセーを発行する回数</param>
	private IObservable<Unit> Repeater(int repeatCount) {
		//Observable.Createを使うことで、メソッドの処理をまとめて遅延評価とする
		return Observable.Create<Unit>(observer => {
			//1秒ごとにOnNextを呼び、repeatCount回OnNextが呼ばれたらOnCompletedを呼んで終了する
			Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
			.Take(repeatCount)					//指定回数でストリーム終了
			.Do(_ => {
				observer.OnNext(Unit.Default);	//メッセージ発行(呼び元のDo、DoOnNext、Subscribeが呼ばれる)
			})
			.DoOnCompleted(() => {
				observer.OnCompleted();			//発行終了(呼び元のDoOnCompletedが呼ばれる)
			})
			.Subscribe();						//タイマー開始

			return Disposable.Create(() => { });
		});
	}

	float a;	//共用のフェードアルファ値(ローカル変数にした方が副作用が無いが、SelectManyのテスト用にあえて外に出してます)
	/// <summary>
	/// フェードアウト
	/// </summary>
	/// <returns>The out as observable.</returns>
	/// <param name="image">Image.</param>
	private IObservable<Unit> fadeOutAsObservable(Image image) {
		a = 0;
		return Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.1))
			.Do(_ => {
				a += 0.01f;
				image.color = new Color(image.color.r, image.color.g, image.color.b, a);
			})
			.TakeWhile(_ => a < 1.0f)
			.Last()
			.AsUnitObservable();
	}
	/// <summary>
	/// フェードイン
	/// </summary>
	/// <returns>The in as observable.</returns>
	/// <param name="image">Image.</param>
	private IObservable<Unit> fadeInAsObservable(Image image) {
		a = 1;
		return Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.1))
			.Do(_ => {
				a -= 0.01f;
				image.color = new Color(image.color.r, image.color.g, image.color.b, a);
			})
			.TakeWhile(_ => a > 0.0f)
			.Last()
			.AsUnitObservable();
	}
	
	private void Start() {
		baseSphere.SetActive(false);

		//画面タップで座標をDebug.Logに表示する
		ScreenClickAsObservable()
			.Subscribe(pos => {
				Debug.LogFormat("Screen Click pos x={0}, y={1}", pos.x, pos.y);
			})
			.AddTo(this);   //AddTo(this)で、thisがDispose(解放)されるとストリームも発行終了する

		//1秒ごとに球を画面内に生成
		Repeater(20)
			.TakeUntilDestroy(this)
			.Subscribe(_ => {
				//球生成
				var cloneSphere = Instantiate(baseSphere, gameObject.transform);
				cloneSphere.transform.position = new Vector3(0, 4, 0);
				cloneSphere.gameObject.SetActive(true);
			});
		
		//1秒以上の長押しクリックで新しい球を画面内に生成
		var eventTrigger = makeSphereButton.gameObject.AddComponent<ObservableEventTrigger>();
		eventTrigger
			.OnPointerDownAsObservable()								//ボタン押下
			.Throttle(TimeSpan.FromSeconds(1))							//1秒後に最後のイベントを下に流す
			.TakeUntil(makeSphereButton.OnPointerExitAsObservable())	//ボタンからマウスが離れてたら中断
			.TakeUntil(makeSphereButton.OnPointerUpAsObservable())		//ボタン押下してなかったら中断
			.RepeatUntilDestroy(makeSphereButton)						//ボタンが死ぬまでこの判定を繰り返す
			.Subscribe(_ => {
				//上方向に飛ぶ球生成
				var cloneSphere = Instantiate(baseSphere, gameObject.transform);
				cloneSphere.transform.position = new Vector3(0, -4, 0);
				cloneSphere.GetComponent<Rigidbody>().velocity = new Vector3(0, 50, 0);
				cloneSphere.gameObject.SetActive(true);
			});

		//シーンのリロード
		var reloadTrigger = reloadButton.gameObject.AddComponent<ObservableEventTrigger>();
		reloadTrigger
			.OnPointerClickAsObservable()
			.Subscribe(_ => {
				SceneManager.LoadScene("UniRxScene");
			});
		//タイマーカウンタ(ゾンビテスト用)
		var timerCount = 0;
		Observable.Timer(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(2))
			.Subscribe(_ => {
				Debug.Log("TimerCount = " + ++timerCount);
			});

		//フェードイン、アウト
#if false
		//ストリームを繋いでシーケンスにする場合、SelectManyで繋ぐと戻り値が違うストリーム同士が繋げるので汎用的(ストリームの合成)
		Observable.ReturnUnit()
			.SelectMany(_ => fadeInAsObservable(fadeImage))     //フェードイン
			.SelectMany(_ => fadeOutAsObservable(fadeImage))    //フェードアウト
			.RepeatUntilDestroy(fadeImage)						//繰り返す
			.Subscribe();
		//ストリームで合成した各メソッドはSubscribe時に一度評価(関数呼び出し)されるため、以下に書き換えると上手く動かない
		//Observable.ReturnUnit()
			//.SelectMany(fadeInAsObservable(fadeImage))
			//.SelectMany(fadeOutAsObservable(fadeImage))
			//.Repeat()
			//.Subscribe();
#endif
	}
}
