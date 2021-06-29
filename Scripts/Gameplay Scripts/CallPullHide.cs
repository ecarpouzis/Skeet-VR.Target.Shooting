using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallPullHide : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Skeet")
        {
            this.gameObject.SetActive(false);
        }
	}
	
}
