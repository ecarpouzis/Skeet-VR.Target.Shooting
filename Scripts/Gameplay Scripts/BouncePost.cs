using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePost : MonoBehaviour {

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, .3f);
    }

}
