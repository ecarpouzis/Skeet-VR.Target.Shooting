using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiGunPosition : MonoBehaviour
{

    public Transform OculusContainer;
    public Transform ViveContainer;

    void Start()
    {
        if (VRApiManager.c_IsOculus || UnityEngine.VR.VRSettings.loadedDeviceName == "Oculus")
        {
            transform.parent = OculusContainer;
        }
        else
        {
            transform.parent = ViveContainer;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
}
