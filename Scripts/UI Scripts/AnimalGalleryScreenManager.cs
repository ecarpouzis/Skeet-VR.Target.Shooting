using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalGalleryScreenManager : MonoBehaviour {

    public Text scoreReadout;
    public Text timeReadout;

    public AnimalGalleryManager gallery;

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        scoreReadout.text = GameTracker.Instance.Points.ToString();
        timeReadout.text = (gallery.timeToPlay - gallery.timePlayed).SecondsToString();
    }
}
