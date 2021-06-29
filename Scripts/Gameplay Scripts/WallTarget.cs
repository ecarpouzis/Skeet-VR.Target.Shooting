using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTarget : Shootable
{
    public Transform LeftDoor, RightDoor;
    public Transform LeftDoorOpen, LeftDoorClosed, RightDoorOpen, RightDoorClosed;

    public SoundSubClip hitSound;
    public SoundSubClip doorOpenSound;
    public SoundSubClip doorCloseSound;
    BoxCollider myCollider;

    public bool isOpen { get; private set; }
    private float desiredOpen = 1f;
    private float currentOpen = 1f;

    private const int pointValue = 100;

    void Start()
    {
        InstantClose();
    }

    public void InstantClose()
    {
        currentOpen = 0f;
        desiredOpen = 0f;
        LeftDoor.localPosition = LeftDoorClosed.localPosition;
        RightDoor.localPosition = RightDoorClosed.localPosition;
    }

    void Update()
    {
        //Update CurrentOpen
        float newOpen = currentOpen;
        float dt = Time.deltaTime * 4;
        if (currentOpen > desiredOpen)
        {
            newOpen -= dt;
            currentOpen = Mathf.Max(desiredOpen, newOpen, 0f);
        }
        else if (currentOpen < desiredOpen)
        {
            newOpen += dt;
            currentOpen = Mathf.Min(desiredOpen, newOpen, 1f);
        }

        Vector3 newlpos = Vector3.Lerp(LeftDoorClosed.localPosition, LeftDoorOpen.localPosition, currentOpen);
        LeftDoor.localPosition = newlpos;

        Vector3 newrpos = Vector3.Lerp(RightDoorClosed.localPosition, RightDoorOpen.localPosition, currentOpen);
        RightDoor.localPosition = newrpos;
    }

    public override void OnHit()
    {
        if (isOpen)
        {
            GameTracker.Instance.AddPoints(this, pointValue);
            hitSound.Play(0f, .5f);
            Close(0f);
        }
    }

    public void Open(float volume)
    {
        desiredOpen = 1f;
        doorOpenSound.Play(.7f, 1.1f, volume);
        isOpen = true;
    }
    public void DelayClose(float delay)
    {
        GameTracker.Instance.Delay(delay, () =>
        {
            Close(.2f);
        });
    }
    public void Close(float volume)
    {
        desiredOpen = 0f;
        if (this.gameObject.activeInHierarchy) 
            doorCloseSound.Play(0.9f, 1.5f, volume);
        isOpen = false;
    }
}
