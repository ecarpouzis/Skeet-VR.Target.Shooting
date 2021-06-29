using UnityEngine;
using System.Collections.Generic;

public class FreezeFragments : MonoBehaviour
{
    public float timeToFreeze = 1f;
    public float timeSinceStart = 0f;
    public float timeSinceDestroy = 40f;

    private List<Transform> originalTransforms = new List<Transform>();
    private List<Vector3> childPositions = new List<Vector3>();
    private List<Quaternion> childRotations = new List<Quaternion>();
    
    // Use this for initialization
    void OPStart()
    {
        timeSinceStart = 0f;


        originalTransforms = new List<Transform>();
        childPositions = new List<Vector3>();
        childRotations = new List<Quaternion>();
        for (int i = 0; i < transform.childCount; i++)
        {
            var t = transform.GetChild(i);
            originalTransforms.Add(t);
            childPositions.Add(t.localPosition);
            childRotations.Add(t.localRotation);

            var rigidbody = t.gameObject.GetComponent<Rigidbody>();
            if (rigidbody == null)
                rigidbody = t.gameObject.AddComponent<Rigidbody>();

            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;

            var collider = t.gameObject.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }

    void OPDestroy()
    {
        for (int i = 0; i < originalTransforms.Count; i++)
        {
            var t = originalTransforms[i];
            t.localPosition = childPositions[i];
            t.localRotation = childRotations[i];
        }
    }
    
    void Update()
    {
        timeSinceStart += Time.deltaTime;
        if (timeSinceStart > timeToFreeze)
        {
            foreach (Transform t in transform)
            {
                Destroy(t.GetComponent<Rigidbody>());
                var collider = t.gameObject.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    collider.enabled = true;
                }

            }
        }
        if (timeSinceStart > timeSinceDestroy)
        {
            ObjectPool.OPDestroyObject(gameObject);
        }
    }
}
