using UnityEngine;
using System.Collections;

public class DrumLoader : MonoBehaviour
{
    public Tommygun tommygun;

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "TommygunAmmo")
        {
            bool didLoad = tommygun.LoadAmmo();
            if (didLoad)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
