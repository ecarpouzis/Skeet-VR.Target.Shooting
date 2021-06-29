using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumEject : MonoBehaviour {

    public GameObject drumPrefab;

    public void EjectDrum()
    {
        GameObject drum = Instantiate(drumPrefab, transform.position, transform.rotation) as GameObject;
        drum.name = "SpentDrum";
        SelfDestruct destruct = drum.AddComponent<SelfDestruct>();
        destruct.givenTime = 6;
        //drum.GetComponent<Rigidbody>().AddForce(transform.right * 100f);
    }

}
