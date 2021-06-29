using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBall : Shootable
{
    //MetalHitSound hitSound;
    float force = 1000f;
    Rigidbody Rigidbody;
    public int bounces = 0;
    BounceBallManager ballManager;
    SoundSubClip hitSound;

    void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        ballManager = GameObject.Find("BounceBallManager").GetComponent<BounceBallManager>();
        Rigidbody.useGravity = false;
        hitSound = GetComponentInChildren<SoundSubClip>();
    }

    override public void OnHit()
    {
        bounces++;
        Rigidbody.useGravity = true;
        Vector2 newDirection = Random.insideUnitCircle.normalized;
        Rigidbody.AddForce((transform.up * (force/1.5f)) + (new Vector3(newDirection.x, 0, newDirection.y) * (force/1.5f)) );
        hitSound.Play(0f, .5f);
        PlayerController.Instance.ControlsDisplay.quickloadText.text = bounces.ToString();
        GameTracker.Instance.AddPoints(this, 1);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            ballManager.EndBounceMode();
            Destroy(gameObject);
        }
    }
}
