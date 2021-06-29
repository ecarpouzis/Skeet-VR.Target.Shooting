using UnityEngine;
using System.Collections;

public class BreakbarrelShellLoader : MonoBehaviour {
    public BreakbarrelShotgun breakbarrelShotgun;
    public bool isLeft = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.name == "BreakbarrelAmmo")
        {
            bool didLoad = breakbarrelShotgun.LoadAmmo(isLeft);
            if (didLoad)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
