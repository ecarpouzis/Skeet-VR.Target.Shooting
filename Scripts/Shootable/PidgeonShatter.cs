using UnityEngine;
using System.Collections;

public class PidgeonShatter : MonoBehaviour {
    
    SoundSubClip pidgeonSound;
    //ParticleSystem confetti;


    public void PlayEffect()
    {
        pidgeonSound.Play(0f, 1.0f);
        //confetti.Play();
    }

    // Use this for initialization
    void OPStart()
    {
        pidgeonSound = this.GetComponent<SoundSubClip>();
        PlayEffect();
        //confetti = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
