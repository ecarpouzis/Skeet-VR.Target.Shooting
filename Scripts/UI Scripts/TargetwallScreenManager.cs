using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetwallScreenManager : MonoBehaviour {

    public Text scoreReadout;
    public Text timeReadout;

    public TargetwallManager wall;

    void Awake()
    {

    }

	// Update is called once per frame
	void Update () {
        scoreReadout.text = GameTracker.Instance.Points.ToString();
        timeReadout.text = (wall.maxTime - wall.timePlaying).SecondsToString();
    }
}
