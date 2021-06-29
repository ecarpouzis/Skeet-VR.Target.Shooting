using UnityEngine;
using System.Collections;

public class GunInfo : MonoBehaviour
{
    public Transform GunUpTransform;

    public Transform TriggerTransform;
    public Transform TriggerUnpulledPosition;
    public Transform TriggerPulledPosition;
    public float TriggerLerpAmt = 0f;

    public Vector3 BarrelForward { get { return GunUpTransform.up * -1f; } }

    void Update()
    {
        if (TriggerTransform != null)
        {
            TriggerTransform.position = Vector3.Lerp(TriggerUnpulledPosition.position, TriggerPulledPosition.position, TriggerLerpAmt);
            TriggerTransform.rotation = Quaternion.Slerp(TriggerUnpulledPosition.rotation, TriggerPulledPosition.rotation, TriggerLerpAmt);
        }
    }
}
