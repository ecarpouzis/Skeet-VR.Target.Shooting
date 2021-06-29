using UnityEngine;
using System.Collections;

public class GlassShatter : MonoBehaviour
{
    SoundSubClip glassSound;

    float[] noiseStartTimes = new float[] { 3.85f, 4.75f, 6.05f, 11.3f };
    float[] noiseEndTimes = new float[] { 4.4f, 5.5f, 7.0f, 12.6f };

    private void Start()
    {
        glassSound = this.GetComponent<SoundSubClip>();
        //Debug.Log(gameObject.name);
    }

    void OPStart()
    {
        int noiseToPlay = Random.Range(0, noiseEndTimes.Length);
        glassSound.Play(noiseStartTimes[noiseToPlay], noiseEndTimes[noiseToPlay]);
    }
}
