﻿using UnityEngine;
using System.Collections;

public class TurnOffControllerModel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
