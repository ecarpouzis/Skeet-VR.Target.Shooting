using UnityEngine;
using System.Collections;

public class ShotgunPickup : MonoBehaviour
{
    public Guns Gun;
    PlayerController player { get { return PlayerController.Instance; } }
    int shotgunLayer = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == shotgunLayer)
        {
            if (!player.isGunOpen)
            {
                player.EquipShotgun(Gun);
            }
        }
    }
}
