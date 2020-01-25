using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectScene : MonoBehaviour
{
	/// <summary>
	/// UnityのInspectorから編集可能な値にするには、SerializeField属性を付けるか、もしくはスコープをpublicにする。
	/// ただ、スコープをpublicにすると、値の初期値が宣言部の代入によるものなのか、Inspectorからの編集によるものなのかが読み取れなくなるため、Inspectorから編集可能な値にスコープpublicを使うべきではない。
	/// スコープがpublicな変数を定義したい場合は、プロパティとして定義してやる事でInspectorからの編集を抑止できる。
	///   例) public int test{ get; set; }
	/// </summary>
	[SerializeField]
	private GameObject cube = null;
	[SerializeField]
	private GameObject noSetCube = null;
	[SerializeField]
	private GameObject dummy = null;

	/// <summary>
	/// インスタンス生成直後に呼ばれる
	/// </summary>
	private void Awake() {
		//UnityのGameObjectにはC#のnullではない、"null"という状態が存在する。
		//Inspectorから代入されていないGameObject、newにより生成されたGameObject(Instantiate以外からの初期化)、完全にDestroyされたGameObjectがこれに該当する。
		//この"null"は==オペレータによる判定は可能だが、??によるnull判定ではnullでないとみなされるため、

		GameObject obj;

		//==オペレータによる判定
		obj = cube == null ? dummy : cube;
		if (obj != null) Debug.Log("[Awake ==] cube name is " + obj.name);
		
		obj = noSetCube == null ? dummy : noSetCube;
		if (obj != null) Debug.Log("[Awake ==] noSetCube name is " + obj.name);

		//null合体演算子による判定
		obj = cube ?? dummy;
		if (obj != null) Debug.Log("[Awake ?? cube name is " + obj.name);
		
		obj = noSetCube ?? dummy;
		if(obj != null) Debug.Log("[Awake ?? noSetCube name is " + obj.name);
	}

	/// <summary>
	/// 初期化完了後に呼ばれる
	/// </summary>
	private void Start() {
		//SceneからGameObjectを破棄するにはDestroyメソッドを呼ぶ。
		//ただ、Destroyは依存関係解決のため解放されるまでに1フレームのディレイがある。
		//どうしても即時解放したい場合はDestroyImmediateを使う。
		//(本件の場合はcubeにnullを代入すればよいように思えるが、他クラスから参照、GameObjectの生存数チェックが行われている場合はそういうわけにもいかない…)
		Destroy(cube);
		//DestroyImmediate(cube);

		GameObject obj;

		//==オペレータによる判定
		obj = cube == null ? dummy : cube;
		if (obj != null) Debug.Log("[Start ==] cube name is " + obj.name);

		obj = noSetCube == null ? dummy : noSetCube;
		if (obj != null) Debug.Log("[Start ==] noSetCube name is " + obj.name);

		//null合体演算子による判定
		obj = cube ?? dummy;
		if (obj != null) Debug.Log("[Start ?? cube name is " + obj.name);

		obj = noSetCube ?? dummy;
		if (obj != null) Debug.Log("[Start ?? noSetCube name is " + obj.name);
	}
}
