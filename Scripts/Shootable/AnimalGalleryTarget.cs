using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalGalleryTarget : Shootable
{
    MetalHitSound hitSound;
    float force = 1000f;
    BoxCollider collider;
    Rigidbody rigidbody;
    int pointsWorth = 100;
    public GameObject correctSound;
    public GameObject wrongSound;
    public bool correctAnimal = false;
    bool stoodUp = false;
    public bool exampleAnimal = false;

    void Awake()
    {
        collider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();
        hitSound = GetComponent<MetalHitSound>();
    }

    void Start()
    {
        collider.enabled = false;
    }

    float timeRising = 0f;
    public void StandUp()
    {
        timeRising += Time.deltaTime;
        Quaternion layingDown = Quaternion.Euler(180f, 0f, 0f);
        Quaternion standing = Quaternion.Euler(270f, 0f, 0f);
        transform.localRotation = Quaternion.Lerp(layingDown, standing, timeRising * 2f);
        if (transform.localRotation.eulerAngles.x == 270f)
        {
            stoodUp = true;
            if (!exampleAnimal)
            {
                collider.enabled = true;
            }
        }
    }

    public void Update()
    {
        if (!stoodUp)
        {
            StandUp();
        }
    }

    override public void OnHit()
    {
        if (!exampleAnimal)
        {
            rigidbody.useGravity = true;
            rigidbody.AddForce(GameObject.Find("MuzzleOut").transform.forward * force);
            if (correctAnimal)
            {
                GameTracker.Instance.AddPoints(this, pointsWorth);
                Instantiate(correctSound, this.transform.position, this.transform.rotation).GetComponent<SoundSubClip>();
            }
            else {
                GameTracker.Instance.AddPoints(this, pointsWorth * -1);
                Instantiate(wrongSound, this.transform.position, this.transform.rotation);
            }
            hitSound.PlayEffect();
            AnimalGalleryManager.Instance.EndSelection();
        }
    }
}
