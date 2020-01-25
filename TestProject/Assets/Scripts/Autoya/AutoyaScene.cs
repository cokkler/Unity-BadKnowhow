using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class AutoyaScene : MonoBehaviour
{
	[SerializeField]
	private ResourceManager resourceManager = null;
	[SerializeField]
	private Image testImage = null;
	
	private void Start() {
		resourceManager.LoadAssetBundleAsync<Sprite>("Assets/AB/Arkanoid/Images/ball.png")
			.Subscribe(sp => {
				testImage.sprite = sp;
			});
	}
}
