using UnityEngine;
using System.Collections;

public class PigShatter : MonoBehaviour {
    public SoundSubClip pigSound;

    void Start()
    {
        pigSound.Play(2.7f, 3.2f);   
    }


}
