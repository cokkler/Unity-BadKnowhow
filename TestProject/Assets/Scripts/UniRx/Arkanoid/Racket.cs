using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racket : MonoBehaviour {
	public float speed;
	
	private void FixedUpdate() {
		float h = Input.GetAxisRaw("Horizontal");
		GetComponent<Rigidbody2D>().velocity = Vector2.right * h * speed;
	}
}
