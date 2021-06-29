using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreditsMusic : MonoBehaviour {
    public List<AudioSource> DanceMagicMusic;
    int currentSong = 0;

    void Start()
    {
        DanceMagicMusic[currentSong].Play();
    }

    void NextSong()
    {
        currentSong++;
        if (currentSong > DanceMagicMusic.Count)
        {
            currentSong = 0;
        }
        DanceMagicMusic[currentSong].Play();
    }

    void Update()
    {
        if (!DanceMagicMusic[currentSong].isPlaying)
        {
            NextSong();
        }
    }

}
