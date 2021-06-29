using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Guns CurrentGun { get; private set; }
    //private string ActiveGun = "";
    public Shotgun shotgun;
    public Slide shotgunSlide;
    public GameTracker gameTracker;

    public ViveRig ViveRig;
    public OculusVRRig OculusRig;

    [HideInInspector]
    public BreakbarrelAmmoSpawner AmmoSpawner { get { if (VRApiManager.c_IsOculus) return OculusRig.AmmoSpawner; else if (VRApiManager.c_IsOpenVR) return ViveRig.AmmoSpawner; return null; } }

    public BreakbarrelShotgun breakbarrelShotgun;
    public Tommygun tommyGun;
    public BreakbarrelShotgun overUnderShotgun;
    public bool isBreakbarrel = false;

    public ControlsDisplay ControlsDisplay { get { if (VRApiManager.c_IsOculus) return OculusRig.ControlsDisplay; else if (VRApiManager.c_IsOpenVR) return ViveRig.ControlsDisplay; return null; } }

    float minTimeBetweenActions = .1f;
    float timeBetweenActions = 0;
    float minTimeBetweenGunpickups = .5f;
    float timeBetweenGunPickups = 0f;
    public bool isGunOpen = false;
    public bool canPull = false;
    public bool CanTeleport = true;
    public Vector3 deltaRot;

    public Teleporter Teleporter { get; private set; }
    public PullShouts PullShouts { get; private set; }
    public Rigidbody heldGun { get; private set; }
    private DynamicVRApi.ControlsApi Controls { get { return VRApiManager.Controls; } }
    private bool prevOculusEnabled = false;

    private void Awake()
    {
        UnityEngine.Application.runInBackground = true;
        Instance = this;
        PullShouts = GetComponent<PullShouts>();
        Teleporter = GetComponent<Teleporter>();

        if (VRApiManager.c_IsOpenVR)
        {
            Teleporter.OriginTransform = ViveRig.transform;
            Teleporter.HeadTransform = ViveRig.Head;
        }
        else if (VRApiManager.c_IsOculus)
        {
            Teleporter.OriginTransform = OculusRig.transform;
            Teleporter.HeadTransform = OculusRig.HeadTrackedObj.transform;
        }
    }
    private void Start()
    {
        CurrentGun = (Guns)(-1);
    }


    Coroutine HapticCoroutineObj;
    IEnumerator HapticCoroutine(float timeToPulse, Controller c, float basePulse)
    {
        float dt = 0;
        while (dt < timeToPulse)
        {
            VRApiManager.Controls.GetController(c).TriggerHaptics(basePulse);
            yield return new WaitForEndOfFrame();
            dt += Time.deltaTime;
        }
        HapticCoroutineObj = null;
    }
    public void TriggerHaptics(float timeToPulse, Controller c, float forceToPulse = 1)
    {
        if (HapticCoroutineObj != null)
        {
            StopCoroutine(HapticCoroutineObj);
        }
        HapticCoroutineObj = StartCoroutine(HapticCoroutine(timeToPulse, c, forceToPulse));
    }

    // Update is called once per frame
    //This is in FixedUpdate in SteamVR_TestThrow
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        bool comp = OVRManager.instance.isUserPresent;
        if (comp != prevOculusEnabled)
        {
            UnityEngine.VR.VRSettings.enabled = comp;
        }
        prevOculusEnabled = comp;

        float triggerLerp = 0f;
        if (Controls.Right.IsActive())
        {
            triggerLerp = Controls.Right.GetAxis1D(Axis1D.GunTriggerAxis);
            if (heldGun != null)
            {
                heldGun.GetComponent<GunInfo>().TriggerLerpAmt = triggerLerp;
            }
        }

        timeBetweenGunPickups += Time.deltaTime;
        timeBetweenActions += Time.deltaTime;
        if (timeBetweenActions > minTimeBetweenActions)
        {
            if (Controls.IsControllerActive(Controller.Left))
            {
                if (Controls.Left.GetButtonDown(Buttons.SpawnAmmo))
                {
                    switch (CurrentGun)
                    {
                        case Guns.PumpAction:
                            shotgunSlide.Grab(Controls.Left.TrackedGameObject);
                            break;
                        case Guns.Tommygun:
                        case Guns.OverUnder:
                        case Guns.SideBySide:
                            AmmoSpawner.SpawnAmmo();
                            break;
                        default:
                            break;
                    }
                    timeBetweenActions = 0f;
                }
                if (Controls.Left.GetButtonUp(Buttons.SpawnAmmo))
                {
                    if (CurrentGun == Guns.PumpAction)
                    {
                        shotgunSlide.LetGo();
                    }
                    timeBetweenActions = 0f;
                }
                if (!Controls.Left.GetButton(Buttons.SpawnAmmo))
                {
                    AmmoSpawner.ReleaseAmmo(Controls.Left.GetVelocity(), Controls.Left.GetAngularVelocity());
                }
                if (Controls.Left.GetButtonDown(Buttons.ToggleControls))
                {
                    ControlsDisplay.ShowControls(!ControlsDisplay.controlsActive);
                    timeBetweenActions = 0f;
                }

                if (Controls.Left.GetButtonDown(Buttons.CallPull) && canPull)
                {
                    gameTracker.SkeetPull();
                    timeBetweenActions = 0f;
                }

            }
            if (Controls.IsControllerActive(Controller.Right))
            {

                if ((int)CurrentGun < 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Credits")
                    EquipShotgun(Guns.PumpAction);

                if (Controls.Right.GetButtonDown(Buttons.FireGun))
                {
                    switch (CurrentGun)
                    {
                        case Guns.PumpAction:
                            shotgun.Shoot();
                            break;
                        case Guns.Tommygun:
                            tommyGun.StartShoot();
                            break;
                        case Guns.SideBySide:
                            breakbarrelShotgun.Shoot();
                            break;
                        case Guns.OverUnder:
                            overUnderShotgun.Shoot();
                            break;
                        default:
                            break;
                    }
                    timeBetweenActions = 0f;
                }

                if (Controls.Right.GetButtonDown(Buttons.ReleaseEmptyAmmo))
                {
                    if (!isGunOpen)
                    {
                        switch (CurrentGun)
                        {
                            case Guns.SideBySide:
                                breakbarrelShotgun.OpenBarrel();
                                break;
                            case Guns.OverUnder:
                                overUnderShotgun.OpenBarrel();
                                break;
                            default:
                                break;
                        }
                        timeBetweenActions = 0f;
                    }
                }

                //Detecting when to stop shooting the Tommygun, when the trigger is released
                if (Controls.Right.GetButtonUp(Buttons.FireGun) || triggerLerp < .1f)
                {
                    if (CurrentGun == Guns.Tommygun)
                    {
                        tommyGun.StopShoot();
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (heldGun != null && Controls.Right.IsActive())
        {
            heldGun.MovePosition(Controls.Right.GetPosition());
            
            Quaternion desiredRotation = Controls.Right.GetRotation();
            Vector3 curRotV = desiredRotation.eulerAngles;

            Vector3 realDeltaRot = deltaRot;// rightController.transform.InverseTransformVector();
            curRotV += realDeltaRot;
            desiredRotation = Quaternion.Euler(curRotV);
            heldGun.MoveRotation(desiredRotation);
        }
    }


    public void EquipShotgun(Guns givenGun)
    {
        AmmoSpawner.ReleaseAmmo(Vector3.zero, Vector3.zero);
        if (timeBetweenGunPickups > minTimeBetweenActions)
        {
            CurrentGun = givenGun;
            overUnderShotgun.bulletCone.clearObjects();
            shotgun.bulletCone.clearObjects();
            breakbarrelShotgun.bulletCone.clearObjects();
            tommyGun.bulletCone.clearObjects();

            MainMenu.Instance.UpdateUI();

            //If the OU is on, turn on the SBS
            switch (givenGun)
            {
                case Guns.SideBySide:
                    EquipSideBySide();
                    break;
                case Guns.PumpAction:
                    EquipPumpAction();
                    break;
                case Guns.OverUnder:
                    EquipOverUnder();
                    break;
                case Guns.Tommygun:
                    EquipTommyGun();
                    break;
                default:
                    Debug.LogError("Given an invalid shotgun:" + givenGun);
                    break;
            }
            timeBetweenGunPickups = 0f;
        }
    }

    private void EquipSideBySide()
    {
        overUnderShotgun.gameObject.SetActive(false);
        shotgun.gameObject.SetActive(false);
        tommyGun.gameObject.SetActive(false);
        breakbarrelShotgun.gameObject.SetActive(true);
        gameTracker.OverUnderPickup.SetActive(true);
        gameTracker.PumpActionPickup.SetActive(true);
        gameTracker.TommygunPickup.SetActive(true);
        gameTracker.SideBySidePickup.SetActive(false);
        breakbarrelShotgun.myRigidbody.MovePosition(Controls.Right.GetPosition());
        breakbarrelShotgun.myRigidbody.MoveRotation(Controls.Right.GetRotation());
        heldGun = breakbarrelShotgun.GetComponent<Rigidbody>();
        heldGun.velocity = Vector3.zero;
        heldGun.angularVelocity = Vector3.zero;
        isBreakbarrel = true;
        ControlsDisplay.SetSBSControls();
        breakbarrelShotgun.CloseBarrel();
    }
    private void EquipPumpAction()
    {
        breakbarrelShotgun.gameObject.SetActive(false);
        overUnderShotgun.gameObject.SetActive(false);
        tommyGun.gameObject.SetActive(false);
        shotgun.gameObject.SetActive(true);
        gameTracker.SideBySidePickup.SetActive(true);
        gameTracker.OverUnderPickup.SetActive(true);
        gameTracker.TommygunPickup.SetActive(true);
        gameTracker.PumpActionPickup.SetActive(false);
        shotgun.myRigidbody.MovePosition(Controls.Right.GetPosition());
        shotgun.myRigidbody.MoveRotation(Controls.Right.GetRotation());
        heldGun = shotgun.GetComponent<Rigidbody>();
        heldGun.velocity = Vector3.zero;
        heldGun.angularVelocity = Vector3.zero;
        isBreakbarrel = false;
        ControlsDisplay.SetPumpControls();
    }
    private void EquipOverUnder()
    {
        breakbarrelShotgun.gameObject.SetActive(false);
        shotgun.gameObject.SetActive(false);
        tommyGun.gameObject.SetActive(false);
        overUnderShotgun.gameObject.SetActive(true);
        gameTracker.SideBySidePickup.SetActive(true);
        gameTracker.PumpActionPickup.SetActive(true);
        gameTracker.TommygunPickup.SetActive(true);
        gameTracker.OverUnderPickup.SetActive(false);
        overUnderShotgun.myRigidbody.MovePosition(Controls.Right.GetPosition());
        overUnderShotgun.myRigidbody.MoveRotation(Controls.Right.GetRotation());
        heldGun = overUnderShotgun.GetComponent<Rigidbody>();
        heldGun.velocity = Vector3.zero;
        heldGun.angularVelocity = Vector3.zero;
        isBreakbarrel = true;
        ControlsDisplay.SetOUControls();
        overUnderShotgun.CloseBarrel();
    }
    private void EquipTommyGun()
    {
        breakbarrelShotgun.gameObject.SetActive(false);
        overUnderShotgun.gameObject.SetActive(false);
        shotgun.gameObject.SetActive(false);
        tommyGun.gameObject.SetActive(true);
        gameTracker.SideBySidePickup.SetActive(true);
        gameTracker.OverUnderPickup.SetActive(true);
        gameTracker.PumpActionPickup.SetActive(true);
        gameTracker.TommygunPickup.SetActive(false);
        tommyGun.myRigidbody.MovePosition(Controls.Right.GetPosition());
        tommyGun.myRigidbody.MoveRotation(Controls.Right.GetRotation());
        heldGun = tommyGun.GetComponent<Rigidbody>();
        heldGun.velocity = Vector3.zero;
        heldGun.angularVelocity = Vector3.zero;
        isBreakbarrel = false;
        ControlsDisplay.SetTommygunControls();
    }
}
