using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BreakbarrelShotgun : MonoBehaviour
{
    public GameObject shellPositionLeft;
    public GameObject shellPositionRight;
    public GameObject shellPrefab;
    public GameObject loadedShellLeft;
    public GameObject loadedShellRight;
    public Rigidbody myRigidbody;
    public Rigidbody buttRigidbody;
    PlayerController player { get { return PlayerController.Instance; } }
    public BulletCollider bulletCone;
    public SoundSubClip shotSound, reloadSound, dryfireSound, barrelCloseClick;
    public ParticleSystem bullets;
    public ParticleSystem muzzleFlash;
    float timeBetweenShots = 0f;
    float minTimeBetweenShots = 0f;
    bool canShoot = false;
    int loadedAmmo;
    int ammoPerLoad = 1;
    public Rigidbody barrelBody;
    public HingeJoint barrelJoint;
    float minBarrelLimit = 0f;
    float maxBarrelLimit = 40f;
    public bool isOpenBarrel = false;
    public bool hasBarrelOpened = false;

    float timeBetweenAmmo = 0f;
    float minTimeBetweenAmmo = .2f;

    private Vector3 hjAnchor;
    private Vector3 hjAxis;
    private bool hjAutoConfigure;
    private Vector3 hjConnectedAnchor;
    private bool hjUseLimits;
    private JointLimits hjLimits;

    private Vector3 originalBarrelPosition;
    private Quaternion originalBarrelRotation;
    private bool allowGunClose = true;
    private GunInfo gunInfo;


    // Use this for initialization
    void Awake()
    {
        gunInfo = GetComponent<GunInfo>();
        minTimeBetweenShots = .1f;

        hjAnchor = barrelJoint.anchor;
        hjAxis = barrelJoint.axis;
        hjAutoConfigure = barrelJoint.autoConfigureConnectedAnchor;
        hjConnectedAnchor = barrelJoint.connectedAnchor;
        hjUseLimits = barrelJoint.useLimits;
        hjLimits = barrelJoint.limits;
        Destroy(barrelJoint);

        barrelBody.isKinematic = true;
        hasBarrelOpened = false;
        isOpenBarrel = false;
        player.isGunOpen = false;
        barrelBody.useGravity = false;

        originalBarrelPosition = barrelBody.transform.localPosition;
        originalBarrelRotation = barrelBody.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        timeBetweenAmmo += Time.deltaTime;
        timeBetweenShots += Time.deltaTime;
        if (timeBetweenShots > minTimeBetweenShots)
        {
            canShoot = true;
        }

        if (allowGunClose)
        {
            //We want to keep the barrel closed if you've snapped it below 5deg.
            //But we don't want to do this the first time the barrel is dropping.
            //So, keep track of if the barrel has been opened and seen above 5deg,
            //and if so, allow for snap-closed.


            if (isOpenBarrel)
            {
                //Barrels go wonky when locked, so attempt to lock when it's almost closed.
                if (barrelJoint.angle <= 0.05f && barrelJoint.angle >= -0.05f && !isOpenBarrel)
                {
                    barrelBody.isKinematic = true;
                }
                if (barrelJoint.angle > 5 && isOpenBarrel && !hasBarrelOpened)
                {
                    hasBarrelOpened = true;
                }
                if (barrelJoint.angle < 5 && isOpenBarrel && hasBarrelOpened)
                {
                    CloseBarrel();
                }
            }
        }
    }

    public bool LoadAmmo(bool isLeft)
    {
        if (isOpenBarrel && timeBetweenAmmo > minTimeBetweenAmmo)
        {
            switch (loadedAmmo)
            {
                case 0:
                    reloadSound.Play(.3f, .6f);
                    loadedAmmo += 1;
                    if (isLeft)
                    {
                        loadedShellLeft.SetActive(true);
                    }
                    else
                    {
                        loadedShellRight.SetActive(true);
                    }
                    timeBetweenAmmo = 0f;
                    return true;
                case 1:
                    loadedShellLeft.SetActive(true);
                    loadedShellRight.SetActive(true);
                    reloadSound.Play(.3f, .6f);
                    loadedAmmo += 1;
                    timeBetweenAmmo = 0f;
                    return true;
                default:
                    return false;
            }
        }
        return false;
    }

    private IEnumerator EjectShells()
    {
        yield return new WaitForSeconds(.09f);
        float shellEjectVelocity = 170f;
        reloadSound.Play(0f, .25f);
        if (loadedShellLeft.activeSelf)
        {
            loadedShellLeft.SetActive(false);
            GameObject shell = Instantiate(shellPrefab, shellPositionLeft.transform.position, shellPositionLeft.transform.rotation) as GameObject;
            shell.GetComponent<Rigidbody>().AddForce(shell.transform.up * shellEjectVelocity);
            var g = shell.AddComponent<SelfDestruct>();
            g.givenTime = 3f;

            if (player.CurrentGun == Guns.OverUnder)
            {
                shell.transform.localScale = new Vector3(.7f, 1.2f, .7f);
            }
        }
        if (loadedShellRight.activeSelf)
        {
            loadedShellRight.SetActive(false);
            GameObject shell = Instantiate(shellPrefab, shellPositionRight.transform.position, shellPositionRight.transform.rotation) as GameObject;
            shell.GetComponent<Rigidbody>().AddForce(shell.transform.up * shellEjectVelocity);
            var g = shell.AddComponent<SelfDestruct>();
            g.givenTime = 3f;
            if (player.CurrentGun == Guns.OverUnder)
            {
                shell.transform.localScale = new Vector3(.7f, 1.2f, .7f);
            }
        }
        loadedAmmo = 0;
    }

    void FixedUpdate()
    {
        if (!isOpenBarrel)
        {
            barrelBody.transform.localRotation = originalBarrelRotation;
            barrelBody.transform.localPosition = originalBarrelPosition;
        }
        else
        {
            Vector3 curPos = barrelBody.transform.localPosition;
            curPos.x = 0f;
            barrelBody.transform.localPosition = curPos;

            Quaternion curRot = barrelBody.transform.localRotation;
            Vector3 newRot = curRot.eulerAngles;
            newRot.z = 0f;
            newRot.y = 0f;
            barrelBody.transform.localRotation = Quaternion.Euler(newRot);
        }
    }

    public void OpenBarrel()
    {
        //Barrel Haptic:
        player.TriggerHaptics(.1f, Controller.Right, 0);

        barrelJoint = barrelBody.gameObject.AddComponent<HingeJoint>();
        barrelJoint.anchor = hjAnchor;
        barrelJoint.axis = hjAxis;
        barrelJoint.autoConfigureConnectedAnchor = hjAutoConfigure;
        barrelJoint.connectedAnchor = hjConnectedAnchor;
        barrelJoint.useLimits = hjUseLimits;
        barrelJoint.limits = hjLimits;

        barrelBody.isKinematic = false;
        JointLimits newLimits = new JointLimits();
        newLimits.max = maxBarrelLimit;
        newLimits.min = minBarrelLimit;
        barrelJoint.limits = newLimits;
        barrelJoint.connectedBody = buttRigidbody;
        isOpenBarrel = true;
        barrelBody.useGravity = true;
        barrelBody.AddForceAtPosition(gunInfo.GunUpTransform.forward * -1f * 25f, gunInfo.GunUpTransform.position);
        player.isGunOpen = true;
        allowGunClose = false;
        StartCoroutine(EjectShells());
        Delay(.2f, () =>
        {
            allowGunClose = true;
        });
    }

    private void Delay(float f, System.Action e)
    {
        StartCoroutine(DelayInternal(f, e));
    }
    private IEnumerator DelayInternal(float f, System.Action e)
    {
        yield return new WaitForSeconds(f);
        e();
    }

    public void CloseBarrel()
    {

        //Barrel Haptic:
        player.TriggerHaptics(.1f, Controller.Right, 0);

        //JointLimits newLimits = new JointLimits();
        //newLimits.max = 0;
        //newLimits.min = 0;
        //barrelJoint.limits = newLimits;
        //barrelJoint.breakForce = 10000000f;
        //barrelJoint.breakTorque = 10000000f;
        //barrelJoint.connectedBody = null;
        Destroy(barrelJoint);

        barrelBody.transform.localRotation = originalBarrelRotation;
        barrelBody.transform.localPosition = originalBarrelPosition;
        barrelBody.isKinematic = true;
        hasBarrelOpened = false;
        isOpenBarrel = false;
        player.isGunOpen = false;
        barrelBody.useGravity = false;
        reloadSound.Play(0f, .25f);
    }

    public void Shoot()
    {
        if (loadedAmmo > 0 && canShoot && !isOpenBarrel)
        {
            //Shotgun Haptic Shoot:
            player.TriggerHaptics(.1f, Controller.Right, 1);

            muzzleFlash.Play();
            bullets.Play();
            shotSound.Play(0f, 1f);
            List<Shootable> shotObjects = bulletCone.seenObjects;
            int clayPidgeonCount = 0;
            if (shotObjects.Count > 0)
            {
                foreach (Shootable shootable in shotObjects)
                {
                    if (shootable != null)
                    {
                        if (shootable is ClayPidgeon)
                        {
                            clayPidgeonCount++;
                            if (clayPidgeonCount == 2)
                                VRApiManager.Achievements.GrantAchievement(Achievements.DAMN_FINE);
                        }
                        shootable.OnHit();
                    }
                }
                bulletCone.clearObjects();
            }
            loadedAmmo--;
            canShoot = false;
            timeBetweenShots = 0f;
        }
        else
        {
            //Dryfire
            dryfireSound.Play();
        }
    }
}
