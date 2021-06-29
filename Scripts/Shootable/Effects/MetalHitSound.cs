using UnityEngine;
using System.Collections;

public class MetalHitSound : MonoBehaviour
{

    SoundSubClip metalSound;

    float[] noiseStartTimes = new float[] { 4.5f, 34.85f,  43.1f };
    float[] noiseEndTimes = new float[] { 7f, 39f, 48f };

    public void PlayEffect()
    {
        int noiseToPlay = Random.Range(0, noiseEndTimes.Length);
        metalSound.Play(noiseStartTimes[noiseToPlay], noiseEndTimes[noiseToPlay]);
    }

    // Use this for initialization
    void Start()
    {
        metalSound = this.GetComponent<SoundSubClip>();
    }

}
