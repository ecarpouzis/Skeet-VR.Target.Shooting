using UnityEngine;
using System.Collections;

public class FireworksManager : MonoBehaviour {
    public ParticleSystem Fireworks1;
    public ParticleSystem Fireworks2;
    SoundSubClip Sound;
    float timeToPlayFireworks = 13.5f;
    float timePlayingFireworks = 0f;

    float timeToPlayFireworksAudio = 15.7f;
    float timePlayingFireworksAudio = 0f;

    void Start()
    {
        Sound = GetComponent<SoundSubClip>();
    }

    public void StartFireworks()
    {
        Fireworks1.Play();
        Fireworks2.Play();
        Sound.Play();
    }

    public void StopFireworks()
    {
        Fireworks1.Stop();
        Fireworks2.Stop();
    }

    void Update()
    {
        timePlayingFireworks += Time.deltaTime;
        timePlayingFireworksAudio += Time.deltaTime;
        if (timePlayingFireworks > timeToPlayFireworks)
        {
            StopFireworks();
            timePlayingFireworks = 0f;
        }
        if (timePlayingFireworksAudio > timeToPlayFireworksAudio)
        {
            Sound.Stop();
            timePlayingFireworksAudio = 0f;
        }
    }
}
