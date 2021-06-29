using UnityEngine;
using System.Collections;

public class MetalTarget : Shootable
{
    MetalHitSound hitSound;
    float force = 1000f;
    Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        hitSound = GetComponent<MetalHitSound>();
    }

    override public void OnHit()
    {
        rigidbody.useGravity = true;
        rigidbody.AddForce(GameObject.Find("MuzzleOut").transform.forward * force);
        hitSound.PlayEffect();
    }
}
