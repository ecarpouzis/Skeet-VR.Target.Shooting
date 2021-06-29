using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PullShouts : MonoBehaviour {
    public List<AudioSource> shouts;

    public void CallPull()
    {
        int randomShout = Random.Range(0,shouts.Count);
        shouts[randomShout].Play();
    }

}
