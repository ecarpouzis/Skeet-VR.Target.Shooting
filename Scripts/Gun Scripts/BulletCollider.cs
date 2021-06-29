using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletCollider : MonoBehaviour
{

    public List<Shootable> seenObjects;
    public Transform pelletOriginPoint;

    public void clearObjects()
    {
        seenObjects.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        var c = other.GetComponents<Shootable>();
        if (c != null)
        {
            seenObjects.AddRange(c);
        }
    }


    void OnTriggerExit(Collider other)
    {
        var c = other.GetComponents<Shootable>();
        if (c != null)
        {
            foreach (Shootable s in c)
                seenObjects.Remove(s);
        }
    }
}
