using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shootable : MonoBehaviour
{
    PlayerController player { get { return PlayerController.Instance; } }
    GunInfo shotgun;
    public Transform pointDisplayOverwrite;
    public string FragmentPrefabPath;

    virtual public void OnHit()
    {
        if (shotgun == null)
        {
            TryFindShotgun();
        }
        if (shotgun != null)
        {
            if (FragmentPrefabPath != null)
            {
                GameObject fragmentContainer = ObjectPool.OPGetObject(FragmentPrefabPath);
                fragmentContainer.transform.position = transform.position;
                fragmentContainer.transform.rotation = transform.rotation;

                if (gameObject.GetComponent<DestroyableSign>() != null)
                {
                    fragmentContainer.transform.localScale = transform.localScale;
                }

                foreach (Transform fragment in fragmentContainer.transform)
                {
                    Rigidbody fragmentBody = fragment.GetComponent<Rigidbody>();
                    if (fragmentBody != null && shotgun != null)
                    {
                        float force = 750f;
                        force += Random.Range(0f, 1000f);
                        fragmentBody.AddForce(shotgun.BarrelForward * force);
                    }
                }
            }
            else
            {
                Debug.LogWarning("FragmentPrefabPath is null, not spawning fragments");
            }
        }
        Destroy(gameObject);
    }

    private void TryFindShotgun()
    {
        shotgun = player.heldGun.GetComponent<GunInfo>();
    }
}