using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitter : MonoBehaviour {
	private Camera cam;

	private float width = 480;
	private float height = 800;
	private float pixelPerUnit = 2;

	void Awake() {
		float aspect = (float)Screen.height / (float)Screen.width;
		float bgAcpect = height / width;

		cam = GetComponent<Camera>();
		cam.orthographicSize = height / 2f / pixelPerUnit;

		if (bgAcpect > aspect) {
			float bgScale = height / Screen.height;
			float camWidth = width / (Screen.width * bgScale);
			cam.rect = new Rect((1.0f - camWidth) / 2.0f, 0.0f, camWidth, 1.0f);
		}
		else {
			float bgScale = aspect / bgAcpect;
			cam.orthographicSize *= bgScale;
			cam.rect = new Rect(0f, 0f, 1f, 1f);
		}
	}
}
