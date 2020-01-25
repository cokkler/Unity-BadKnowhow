using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;

public class Arkanoid : MonoBehaviour
{
	[SerializeField]
	private Ball ball = null;			//ボール
	[SerializeField]
	private Racket racket = null;		//ラケット
	[SerializeField]
	private GameObject stage = null;	//ステージ(子にステージを構成するブロックが並んでいる)
	[SerializeField]
	private Text scoreText = null;		//スコアテキスト
	[SerializeField]
	private Text highScoreText = null;	//ハイスコアテキスト
	[SerializeField]
	private Text gameoverText = null;	//ゲームオーバーテキスト
	[SerializeField]
	private Text gameclearText = null;	//ゲームクリアテキスト

	private void Start() {
		var score = 0;
		var blockNum = stage.GetComponentsInChildren<Block>().Length;

		//UniRx練習用シーン
		//UniRxのみを使い、かつStartメソッドのみにTODOの判定を記述せよ

		//Update毎に呼ばれるイベントを作成する場合は、Observable.EveryUpdate を使う
		//例)
		//Observable.EveryUpdate()
		//	.Where(_ => メソッドチェインの下に続く条件 trueなら下に続く)
		//  .Subscribe(_ => {
		//     条件を満たした場合の処理を書く
		//  });
		//
		//Subscribeに達するまでの条件をメソッドチェインで書いていくことになる

		//TODO:ボールのY座標が-120を超えたらゲームオーバーにする
		//HINT:ボールの座標はball.transform.position参照、ゲームオーバーテキストをActiveにする

		//TODO:ブロックの数が減る毎にスコア+100する & ボールのスピードを消した数分増やす
		//HINT:ブロックの数はstage.transform.childCount、ボールのスピードはball.GetComponent<Rigidbody2D>().velocity

		//TODO:ブロックの数が0になったらゲームクリアにする
		//HINT:ゲームクリアテキストをActiveにする

		//TODO:ゲームオーバー、ゲームクリアの5秒後にゲームを再開する ハイスコアを記録すること
		//     SceneManager.LoadScene("シーン名") でシーンのリロードができる
		//HINT:static変数などでハイスコア保存し、開始時にハイスコアテキストに反映させる
	}
}
