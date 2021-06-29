using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusVRRig : MonoBehaviour
{
    public GameObject RightTrackedObj, LeftTrackedObj, HeadTrackedObj;
    public BreakbarrelAmmoSpawner AmmoSpawner;
    public ControlsDisplay ControlsDisplay;

    private void Awake()
    {
        VRApiManager.OculusControls.SetControllerManager(this);
    }
}
