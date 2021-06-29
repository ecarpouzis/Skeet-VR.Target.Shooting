using UnityEngine;
using System.Collections;

public class SoundSubClip : MonoBehaviour {
    AudioSource thisClip;
    float endTime;
    bool countDown = false;

	// Use this for initialization
	void Awake () {
        thisClip = gameObject.GetComponent<AudioSource>();
    }

    //public float loopStart = 0f, loopEnd = 1f;
    //public void Loop()
    //{
    //    if (!thisClip.isPlaying)
    //    {
    //        Play(loopStart, loopEnd);
    //    }
    //}
    

    public void Play()
    {
        thisClip.Stop();
        thisClip.Play();
    }
    
    public void Play(float startTime, float givenEnd, float givenVolume)
    {
        thisClip.volume = givenVolume;
        thisClip.Stop();
        thisClip.time = startTime;
        endTime = givenEnd;
        //Debug.Log("Playing :" + thisClip.name + " Volume:" + thisClip.volume+" Time: "+thisClip.time);
        thisClip.Play();
    }

    public void Play(float startTime)
    {
        thisClip.Stop();
        thisClip.time = startTime;
        thisClip.Play();
    }

    public void Stop()
    {
        thisClip.Stop();
    }

    public void Play(float startTime, float givenEnd)
    {
        thisClip.Stop();
        thisClip.time = startTime;
        endTime = givenEnd;
        thisClip.Play();
    }


    // Update is called once per frame
    void Update () {
        if (endTime > 0)
        {
            countDown = true;
        }
        if (countDown)
        {
            if (thisClip.time > endTime)
            {
                thisClip.Stop();
            }
        }
    }
}
