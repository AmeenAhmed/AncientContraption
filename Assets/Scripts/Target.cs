﻿using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {
	public GameObject src;
	public GameObject main;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.gameObject.name == "source") {
			main.GetComponent<Main> ().targetHit ();
		}
	}
}
