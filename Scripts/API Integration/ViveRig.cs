using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveRig : MonoBehaviour {

    public SteamVR_ControllerManager ControllerManager;
    public Transform Head;
    public BreakbarrelAmmoSpawner AmmoSpawner;
    public ControlsDisplay ControlsDisplay;

    private void Awake()
    {
        VRApiManager.OpenVRControls.SetControllerManager(this);
    }
}
