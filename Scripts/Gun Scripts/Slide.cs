using UnityEngine;
using System.Collections;

public class Slide : MonoBehaviour
{
    public Shotgun shotgun;
    public Transform SlideFront, SlideBack;

    public Transform ShellEjectCover;
    public Transform ShellEjectCoverBack, ShellEjectCoverFront;

    private GameObject controller;
    private bool isGrabbed = false;
    private Vector3 firstControllerPos;
    private float slideDiffConstant = 2.4f;
    private bool isBack;

    void Start()
    {
        firstControllerPos = SlideFront.position;
        isBack = false;
    }

    void Update()
    {
        float curZ = transform.localPosition.z;
        float frontZ = SlideFront.localPosition.z;
        float backZ = SlideBack.localPosition.z;
        float diffP = (frontZ - curZ) / (frontZ - backZ); //% diff of Slide's current Z between Slide's front Z and Slide's back Z, diffP of 0 means slide is all the way forward, diffP of 1 means slide is all the way back

        ShellEjectCover.position = Vector3.Lerp(ShellEjectCoverFront.position, ShellEjectCoverBack.position, diffP);

        if (isGrabbed)
        {
            //Store the current position of the controller, transformed into localspace of the slide's transform.
            Vector3 curPos = transform.InverseTransformVector(controller.transform.position);
            Vector3 firstPos = transform.InverseTransformVector(firstControllerPos);

            float diff;
            diff = curPos.x - firstPos.x;

            //Take the current local position of the slide
            Vector3 curSlidePos = SlideFront.localPosition;
            diff *= slideDiffConstant;
            curSlidePos.z -= diff;

            if (curSlidePos.z > frontZ)
            {
                curSlidePos.z = frontZ;
                firstControllerPos = controller.transform.position;
            }
            else if (curSlidePos.z < backZ)
            {
                curSlidePos.z = backZ;
                firstControllerPos = controller.transform.position + (1.1f * (SlideFront.position - SlideBack.position));
            }

            transform.localPosition = curSlidePos;
        }
        else
        {
            Vector3 newVector = transform.localPosition;
            float dz = frontZ - backZ * Time.deltaTime * 4f;
            newVector.z += dz;
            if (newVector.z >= frontZ)
                newVector.z = frontZ;
            transform.localPosition = newVector;
        }

        if (!isBack)
        {
            if (diffP >= .7f)
            {
                isBack = true;
                shotgun.EjectShell();
            }
        }
        else
        {
            if (diffP <= .3f)
            {
                isBack = false;
                shotgun.LoadAmmo();
            }
        }
    }

    public void LetGo()
    {
        isGrabbed = false;
        controller = null;
    }
    public void Grab(GameObject givenController)
    {
        //Store a reference to the controller
        controller = givenController;

        //Store the starting position of the controller, transformed into localspace from the slide's transform.
        firstControllerPos = controller.transform.position;

        //Set isGrabbed to true, so the slide should start updating
        isGrabbed = true;
    }
}