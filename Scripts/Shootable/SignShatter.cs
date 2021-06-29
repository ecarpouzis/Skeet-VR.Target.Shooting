using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SignShatter : MonoBehaviour {
    public List<SoundSubClip> woodSounds = new List<SoundSubClip>();

    void OPStart()
    {
        int chosenSound = Random.Range(0, woodSounds.Count);
        if(chosenSound == 1){
            woodSounds[chosenSound].Play(.1f);
        } else if (chosenSound == 2)
        {
            woodSounds[chosenSound].Play(.1f);
        }
        else{
            woodSounds[chosenSound].Play();
        }
    }
}
