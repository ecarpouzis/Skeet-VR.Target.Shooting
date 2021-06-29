using UnityEngine;
using System.Collections;

public class ScorePointSound : MonoBehaviour {

	// Use this for initialization
	void OPStart() {
        GetComponent<SoundSubClip>().Play(0f, .5f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
