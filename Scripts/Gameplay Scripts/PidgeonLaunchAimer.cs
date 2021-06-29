using UnityEngine;
using System.Collections;

public class PidgeonLaunchAimer : MonoBehaviour
{
    Transform PidgeonTarget;
    public PidgeonLauncher pidgeonLauncher;

    // Use this for initialization
    void Awake()
    {
        PidgeonTarget = GameObject.Find("PidgeonTarget").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (pidgeonLauncher != null)
        {
            if (pidgeonLauncher.launcherRunning)
            {
                Vector3 targetPoint = new Vector3();
                if (PidgeonTarget != null)
                {
                    targetPoint = new Vector3(PidgeonTarget.position.x, this.transform.position.y, PidgeonTarget.position.z);
                }

                transform.LookAt(targetPoint);
            }
        }
        else
        {
            transform.LookAt(PidgeonTarget);
        }
    }
}
