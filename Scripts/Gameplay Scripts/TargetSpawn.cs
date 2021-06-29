using UnityEngine;
using System.Collections;

public class TargetSpawn : MonoBehaviour {

    public GameObject targetPrefab;
    public Transform endTransform;
    GameObject createdTarget;
    float riseSpeed = 2f;
    float timeSinceStart = 0f;

    public void SpawnTarget()
    {
        createdTarget = Instantiate(targetPrefab, transform.position, transform.rotation) as GameObject;
        createdTarget.gameObject.name = "ArcadeSign";
        timeSinceStart = 0f;
    }

    void Update()
    {
        timeSinceStart += Time.deltaTime;
        if (createdTarget!=null)
        {
            createdTarget.transform.position = Vector3.Lerp(transform.position, endTransform.position, timeSinceStart);
        }
    }

}
