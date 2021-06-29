using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateVRCamera : MonoBehaviour
{
    public GameObject OpenVRContainer;
    public GameObject OculusContainer;

    private void Awake()
    {
        if (VRApiManager.c_IsOculus)
        {
            OpenVRContainer.SetActive(false);
            OculusContainer.SetActive(true);
        }
        else if (VRApiManager.c_IsOpenVR)
        {
            OpenVRContainer.SetActive(true);
            OculusContainer.SetActive(false);
        }
    }
}
