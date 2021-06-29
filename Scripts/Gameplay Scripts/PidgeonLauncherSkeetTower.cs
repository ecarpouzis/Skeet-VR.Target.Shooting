using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PidgeonLauncherSkeetTower : MonoBehaviour
{
    public int pidgeonsLaunched = 0;
    public GameObject pidgeonPrefab;
    public Transform launchTransform;
    float speed = 25f;
    public bool launcherRunning;
    int pidgeonsToShoot = 0;

    // Update is called once per frame
    void Update()
    {

    }

    public void ShootPidgeon(bool lastPidgeon, int skeetModeRound)
    {
        GameObject pidgeon = Instantiate(pidgeonPrefab, launchTransform.position, launchTransform.rotation) as GameObject;
        pidgeon.name = pidgeonPrefab.name;
        pidgeonsLaunched++;
        pidgeon.GetComponent<Rigidbody>().velocity = launchTransform.forward * speed;
        ClayPidgeon script = pidgeon.GetComponent<ClayPidgeon>();
        script.LastPidgeon = lastPidgeon;
        script.SkeetModeRound = skeetModeRound;
    }
}
