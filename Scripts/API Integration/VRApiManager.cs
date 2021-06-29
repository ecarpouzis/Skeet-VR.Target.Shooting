using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicVRApi;
using Oculus.Platform;
using System;
using Oculus.Platform.Models;


#region enums
public enum VRApi { OpenVR, Oculus }
public enum Achievements : int
{
    NUMBER_ONE = 0,
    TOP_THAT = 1,
    BACON = 2,
    DAMN_FINE = 3,
    CLEANING_UP = 4,
    STOP_THE_INVASION = 5,
    NO_ANIMALS_WERE_HARMED = 6,
    WHAT_A_MESS = 7,
    SPIN_ME_ROUND = 8,
    WINNER_WINNER = 9,
    RED_ROOM = 10,
    OFF_THE_WALLS = 11,
    SOME_ANIMALS_WERE_HARMED = 12,
    ANOTHER_BRICK = 13
};
public enum Controller { Left, Right }
public enum Buttons { FireGun, SpawnAmmo, ReleaseEmptyAmmo, ToggleControls, CallPull, Teleport }
public enum Axis1D { GunTriggerAxis }
public enum Axis2D { }

public enum Guns { PumpAction, SideBySide, OverUnder, Tommygun }
public enum Modes { Trap, Skeet, Quickdraw, Bounce, Targetwall, AnimalGallery }
public enum LeaderboardFilter { Global, Friends }

[Flags]
public enum DLCUnlock
{
    None = 0,
    Bronze = 0x001,
    Silver = 0x010,
    Gold = 0x100
}
#endregion

public static class VRApiManager
{
    public static VRApi ControlsApi { get; private set; }
    public static VRApi PlatformApi { get; private set; }

    public static bool c_IsOculus { get { return ControlsApi == VRApi.Oculus; } }
    public static bool c_IsOpenVR { get { return ControlsApi == VRApi.OpenVR; } }
    public static bool p_IsOculus { get { return PlatformApi == VRApi.Oculus; } }
    public static bool p_IsOpenVR { get { return PlatformApi == VRApi.OpenVR; } }
    private static bool oculusInit = false;
    public static bool p_IsInit { get { if (p_IsOculus) return oculusInit; else if (p_IsOpenVR) return SteamManager.Initialized; return false; } }

    public static AchievementsApi Achievements { get { if (p_IsOculus) return OculusAchievements; if (p_IsOpenVR) return SteamAchievements; return null; } }
    public static ControlsApi Controls { get { if (c_IsOculus) return OculusControls; if (c_IsOpenVR) return OpenVRControls; return null; } }
    public static LeaderboardApi Leaderboard { get { if (p_IsOpenVR) return SteamLeaderboard; if (p_IsOculus) return OculusLeaderboard; return null; } }
    public static DLCApi DLC { get { if (p_IsOpenVR) return SteamDLC; if (p_IsOculus) return OculusDLC; return null; } }

    public static OpenVRControls OpenVRControls { get; private set; }
    public static OculusControls OculusControls { get; private set; }

    public static SteamAchievements SteamAchievements { get; private set; }
    public static OculusAchievements OculusAchievements { get; private set; }

    public static SteamLeaderboard SteamLeaderboard { get; private set; }
    public static OculusLeaderboard OculusLeaderboard { get; private set; }

    public static SteamDLC SteamDLC { get; private set; }
    public static OculusDLC OculusDLC { get; private set; }

    public static SteamManager SteamManager { get; private set; }

    private static bool requireEntitlement = true;

    static VRApiManager()
    {
        SteamAchievements = new SteamAchievements();
        OculusAchievements = new OculusAchievements();
        OpenVRControls = new OpenVRControls();
        OculusControls = new OculusControls();
        SteamLeaderboard = new SteamLeaderboard();
        OculusLeaderboard = new OculusLeaderboard();
        SteamDLC = new SteamDLC();
        OculusDLC = new OculusDLC();

        InitializeAPI(VRApi.Oculus);
    }

    private static void InitializeAPI(VRApi releasePlatform)
    {
        if (releasePlatform == VRApi.OpenVR)
        {
            if (UnityEngine.VR.VRSettings.loadedDeviceName == "Oculus")
            {
                SetControls(VRApi.Oculus);
            }
            else
            {
                SetControls(VRApi.OpenVR);
            }
        }
        else
        {
            SetControls(VRApi.Oculus);
        }

        string[] args = System.Environment.GetCommandLineArgs();
        foreach (string s in args)
        {
            if (s.ToLower() == "-oculus")
                SetControls(VRApi.Oculus);
            else if (s.ToLower() == "-openvr")
                SetControls(VRApi.OpenVR);

            if (s.ToLower() == "-noentitlement")
            {
                requireEntitlement = false;
            }
        }

        SetPlatform(releasePlatform);
    }

    public static void SetControls(VRApi api)
    {
        ControlsApi = api;
    }
    public static void SetPlatform(VRApi api)
    {
        PlatformApi = api;

        if (api == VRApi.Oculus)
        {
            bool error = true;
            try
            {
                if (!Core.IsInitialized())
                {
                    Core.Initialize("1203181453071197"); //Oculus App Id
                }
                if (Core.IsInitialized())
                {
                    oculusInit = true;

                    Entitlements.IsUserEntitledToApplication().OnComplete((x) =>
                    {
                        if (x.IsError)
                        {
                            Error e = x.GetError();
                            Debug.Log("Entitlements.IsUserEntitledToApplication() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);

                            if (requireEntitlement)
                            {
                                UnityEngine.Application.Quit();
                            }
                        }
                        else
                        {
                            Debug.Log("Entitlement check passed");
                        }
                    });

                    error = false;
                    Oculus.Platform.Users.GetLoggedInUser().OnComplete((x) =>
                    {
                        if (!x.IsError)
                        {
                            Debug.Log("Welcome " + x.Data.OculusID);
                        }
                        else
                        {
                            Error e = x.GetError();
                            Debug.Log("Users.GetLoggedInUser() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);
                        }
                    });

                    OculusDLC.UpdateCache(() =>
                    {
                        OculusLeaderboard.Initialize();
                    });
                }
            }
            catch { }
            if (error)
                Debug.LogError("Error initializing oculus platform");
        }
        else if (api == VRApi.OpenVR)
        {
            if (SteamManager != null)
            {
                Debug.LogError("SteamManager should be null when we set API Platform to OpenVR");
            }
            else
            {
                GameObject go = new GameObject("SteamManager");
                SteamManager = go.AddComponent<SteamManager>();

                if (SteamManager.Initialized)
                {
                    SteamLeaderboard.Initialize();

                }
            }
        }
    }
}

namespace DynamicVRApi
{
    #region Helper
    public class HSFloatInt
    {
        public int RawVal { get; private set; }
        public int DisplayInt { get { return RawVal / 1000; } set { RawVal = value * 1000; } }
        public float DisplayFloat { get { return ((float)RawVal) / 1000f; } set { RawVal = (int)(value * 1000f); } }

        public HSFloatInt(int rawVal) { this.RawVal = rawVal; }
        public HSFloatInt(float initialF) { DisplayFloat = initialF; }
    }

    public delegate void AsyncApiDelegate<T>(AsyncApiCallback<T> token);
    public delegate void AsyncApiDelegate(AsyncApiCallback token);

    public class AsyncApiCallback<T>
    {
        public Status Status;
        public T Payload;

        public AsyncApiCallback() { }
    }
    public class AsyncApiCallback
    {
        public Status Status;
    }
    internal class MultiCR<T>
    {
        private Dictionary<SteamAPICall_t, CallResult<T>> dict;

        public MultiCR()
        {
            dict = new Dictionary<SteamAPICall_t, CallResult<T>>();
        }

        public void Set(SteamAPICall_t api, CallResult<T>.APIDispatchDelegate OnComplete)
        {
            var g = CallResult<T>.Create();
            g.Set(api, (x, y) => { dict.Remove(api); OnComplete(x, y); });
            dict.Add(api, g);
        }
    }

    public enum Status { NotInitialized, ApiFail, Succeess }
    #endregion
    #region Achievements
    public abstract class AchievementsApi
    {
        private int pigCounter = 0;
        public void PigCounterAdd()
        {
            pigCounter++;
            if (pigCounter == 4)
            {
                GrantAchievement(Achievements.BACON);
            }
        }
        public void PigCounterReset()
        {
            pigCounter = 0;
        }

        protected const int MaxUFOs = 50;

        public void UFOCounterAdd()
        {
            Achievements ach = Achievements.STOP_THE_INVASION;
            string UFOPlayerPrefsKey = "ach" + ach.ToString();
            int i = 0;
            if (PlayerPrefs.HasKey(UFOPlayerPrefsKey))
                i = PlayerPrefs.GetInt(UFOPlayerPrefsKey);
            i++;
            PlayerPrefs.SetInt(UFOPlayerPrefsKey, i);
            PlayerPrefs.Save();
            if (i >= MaxUFOs)
                GrantAchievement(ach);
            else if (i % 5 == 0)
                OnUFOProgress(i);
        }
        protected virtual void OnUFOProgress(int curUFOs) { }

        public abstract void GrantAchievement(Achievements a);

    }
    public class SteamAchievements : AchievementsApi
    {
        public SteamAchievements() { }

        public override void GrantAchievement(Achievements a)
        {
            if (SteamManager.Initialized)
            {
                string n = a.ToString();
                bool achCompleted;
                if (!SteamUserStats.GetAchievement(n, out achCompleted))
                {
                    Debug.LogError("Error granting achievement");
                    return;
                }
                if (!achCompleted)
                {
                    SteamUserStats.SetAchievement(a.ToString());
                    SteamUserStats.StoreStats();
                }
            }
        }
        protected override void OnUFOProgress(int curUFOs)
        {
            SteamUserStats.IndicateAchievementProgress(Achievements.STOP_THE_INVASION.ToString(), (uint)curUFOs, MaxUFOs);
        }
    }
    public class OculusAchievements : AchievementsApi
    {
        public OculusAchievements() { }

        public override void GrantAchievement(Achievements a)
        {
            bool b = Core.IsInitialized();
            if (b)
            {
                string n = a.ToString();
                var req = Oculus.Platform.Achievements.Unlock(n);
                req.OnComplete((x) =>
                {
                    if (a != Achievements.NUMBER_ONE)
                    {
                        if (x.IsError)
                        {
                            var error = x.GetError();
                            Debug.Log("OVR - Achievements.Unlock(" + n + ") error: " + error.Message);
                        }
                        else
                        {
                            Debug.Log("OVR - Granted achievement: " + n);
                        }
                    }
                });
            }
        }
    }
    #endregion
    #region Controls
    public abstract class ControlsApi
    {
        public DynamicApiController Left { get { return GetController(Controller.Left); } }
        public DynamicApiController Right { get { return GetController(Controller.Right); } }

        public bool IsControllerActive(Controller type)
        {
            return GetController(type).IsActive();
        }
        public abstract DynamicApiController GetController(Controller type);
    }
    public class OpenVRControls : ControlsApi
    {
        public ViveRig ViveRig { get; private set; }
        private SteamVR_TrackedObject left, right;
        private OpenVRController RealLeft;
        private OpenVRController RealRight;

        public OpenVRControls() { }

        public override DynamicApiController GetController(Controller type)
        {
            switch (type)
            {
                case Controller.Left:
                    return RealLeft;
                case Controller.Right:
                    return RealRight;
                default:
                    throw new Exception("invalid api");
            }
        }

        //This should only get called from SteamVR_ControllerManager.Awake()
        public void SetControllerManager(ViveRig viveRig)
        {
            ViveRig = viveRig;
            left = ViveRig.ControllerManager.left.GetComponent<SteamVR_TrackedObject>();
            right = ViveRig.ControllerManager.right.GetComponent<SteamVR_TrackedObject>();
            RealLeft = new OpenVRController(left);
            RealRight = new OpenVRController(right);
        }
    }
    public class OculusControls : ControlsApi
    {
        public OculusVRRig VRRig { get; private set; }
        private GameObject left, right;
        private OculusController RealLeft, RealRight;

        public OculusControls() { }

        public override DynamicApiController GetController(Controller type)
        {
            switch (type)
            {
                case Controller.Left:
                    return RealLeft;
                case Controller.Right:
                    return RealRight;
                default:
                    throw new Exception("invalid api");
            }
        }

        //This should only get called from OculusVRRig.Awake()
        public void SetControllerManager(OculusVRRig newRig)
        {
            VRRig = newRig;
            left = newRig.LeftTrackedObj;
            right = newRig.RightTrackedObj;
            RealLeft = new OculusController(OVRInput.Controller.LTouch, left);
            RealRight = new OculusController(OVRInput.Controller.RTouch, right);
        }
    }


    public abstract class DynamicApiController
    {
        public GameObject TrackedGameObject { get; protected set; }

        public abstract bool IsActive();
        public abstract Vector3 GetPosition();
        public abstract Quaternion GetRotation();
        public abstract Vector3 GetVelocity();
        public abstract Vector3 GetAngularVelocity();

        public abstract bool GetButtonDown(Buttons b);
        public abstract bool GetButtonUp(Buttons b);
        public abstract bool GetButton(Buttons b);
        public abstract float GetAxis1D(Axis1D a);
        public abstract Vector2 GetAxis2D(Axis2D a);

        public virtual void TriggerHaptics(float inputStrength) { }//0f = no haptics 1f = max haptics
    }
    public class OpenVRController : DynamicApiController
    {
        private SteamVR_TrackedObject SteamVRObject { get; set; }
        internal SteamVR_Controller.Device Input
        {
            get
            {
                if (SteamVRObject == null || !SteamVRObject.isActiveAndEnabled)
                    return null;

                int i = (int)SteamVRObject.index;
                if (i >= 0)
                    return SteamVR_Controller.Input(i);

                return null;
            }
        }

        public OpenVRController(SteamVR_TrackedObject VRObject)
        {
            SteamVRObject = VRObject;
            TrackedGameObject = VRObject.gameObject;
        }

        public override bool IsActive()
        {
            return SteamVRObject != null && Input != null;
        }

        public override Vector3 GetPosition()
        {
            return SteamVRObject.transform.position;
        }
        public override Quaternion GetRotation()
        {
            return SteamVRObject.transform.rotation;
        }
        public override Vector3 GetVelocity()
        {
            return Input.velocity;
        }
        public override Vector3 GetAngularVelocity()
        {
            return Input.angularVelocity;
        }
        public override void TriggerHaptics(float inputStrength)
        {
            ushort u = (ushort)Mathf.Lerp(500f, 3000f, inputStrength);
            Input.TriggerHapticPulse(u);
        }

        private ulong ConvertButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.FireGun:
                    return SteamVR_Controller.ButtonMask.Trigger;
                case Buttons.ReleaseEmptyAmmo:
                    return SteamVR_Controller.ButtonMask.Touchpad;
                case Buttons.SpawnAmmo:
                    return SteamVR_Controller.ButtonMask.Trigger;
                case Buttons.ToggleControls:
                    return SteamVR_Controller.ButtonMask.Grip;
                case Buttons.CallPull:
                    return SteamVR_Controller.ButtonMask.Touchpad;
                case Buttons.Teleport:
                    return SteamVR_Controller.ButtonMask.Touchpad;
                default:
                    throw new Exception("Invalid button: " + b.ToString());
            }
        }
        public override bool GetButtonDown(Buttons b)
        {
            return Input.GetPressDown(ConvertButton(b));
        }
        public override bool GetButtonUp(Buttons b)
        {
            return Input.GetPressUp(ConvertButton(b));
        }
        public override bool GetButton(Buttons b)
        {
            return Input.GetPress(ConvertButton(b));
        }

        private Valve.VR.EVRButtonId ConvertAxis1D(Axis1D a)
        {
            switch (a)
            {
                case Axis1D.GunTriggerAxis:
                    return Valve.VR.EVRButtonId.k_EButton_Axis1;
                default:
                    throw new Exception("Invalid axis");
            }
        }
        public override float GetAxis1D(Axis1D a)
        {
            return Input.GetAxis(ConvertAxis1D(a)).x;
        }

        private Valve.VR.EVRButtonId ConvertAxis2D(Axis2D a)
        {
            switch (a)
            {
                default:
                    throw new Exception("Invalid axis");
            }
        }
        public override Vector2 GetAxis2D(Axis2D a)
        {
            return Input.GetAxis(ConvertAxis2D(a));
        }

    }
    public class OculusController : DynamicApiController
    {
        private OVRInput.Controller MyController;
        private int HapticsChannel = -1;

        public OculusController(OVRInput.Controller c, GameObject trackedObject)
        {
            MyController = c;
            if (c == OVRInput.Controller.LTouch)
            {
                HapticsChannel = 0;
            }
            else if (c == OVRInput.Controller.RTouch)
            {
                HapticsChannel = 1;
            }
            TrackedGameObject = trackedObject;
        }

        public override bool IsActive()
        {
            return OVRInput.GetControllerOrientationTracked(MyController) && OVRInput.GetControllerPositionTracked(MyController);
        }

        public override Vector3 GetPosition()
        {
            return TrackedGameObject.transform.position;
        }
        public override Quaternion GetRotation()
        {
            return TrackedGameObject.transform.rotation;
        }
        public override Vector3 GetVelocity()
        {
            return VRApiManager.OculusControls.VRRig.transform.TransformVector(OVRInput.GetLocalControllerVelocity(MyController));
        }
        public override Vector3 GetAngularVelocity()
        {
            return VRApiManager.OculusControls.VRRig.transform.TransformVector(OVRInput.GetLocalControllerAngularVelocity(MyController).eulerAngles);
        }
        public override void TriggerHaptics(float inputStrength)
        {
            if (HapticsChannel >= 0 && HapticsChannel < OVRHaptics.Channels.Length)
            {
                const int g = 12;
                int num = (int)Mathf.Lerp(4, 6, inputStrength);
                byte b = (byte)Mathf.Lerp(90, 200, inputStrength);
                byte[] clipData = new byte[num];
                for (int i = 0; i < num; i++)
                {
                    clipData[i] = b;
                }

                var clip = new OVRHapticsClip(clipData, num);
                var haptics = OVRHaptics.Channels[HapticsChannel];
                haptics.Preempt(clip);
            }
        }

        private OVRInput.Button ConvertButton(Buttons b)
        {
            switch (b)
            {
                case Buttons.FireGun:
                    return OVRInput.Button.PrimaryIndexTrigger;
                case Buttons.ReleaseEmptyAmmo:
                    return OVRInput.Button.One;
                case Buttons.SpawnAmmo:
                    return OVRInput.Button.PrimaryHandTrigger | OVRInput.Button.PrimaryIndexTrigger;
                case Buttons.ToggleControls:
                    return OVRInput.Button.PrimaryThumbstick;
                case Buttons.CallPull:
                    return OVRInput.Button.One;
                case Buttons.Teleport:
                    return OVRInput.Button.Two;
                default:
                    throw new Exception("Invalid button");
            }
        }
        public override bool GetButtonDown(Buttons b)
        {
            bool bo = OVRInput.GetDown(ConvertButton(b), MyController);
            return bo;
        }
        public override bool GetButtonUp(Buttons b)
        {
            return OVRInput.GetUp(ConvertButton(b), MyController);
        }
        public override bool GetButton(Buttons b)
        {
            return OVRInput.Get(ConvertButton(b), MyController);
        }

        private OVRInput.Axis1D ConvertAxis1D(Axis1D a)
        {
            switch (a)
            {
                case Axis1D.GunTriggerAxis:
                    return OVRInput.Axis1D.PrimaryIndexTrigger;
                default:
                    throw new Exception("Invalid axis");
            }
        }
        public override float GetAxis1D(Axis1D a)
        {
            return OVRInput.Get(ConvertAxis1D(a), MyController);
        }

        private OVRInput.Axis2D ConvertAxis2D(Axis2D a)
        {
            switch (a)
            {
                default:
                    throw new Exception("Invalid axis");
            }
        }
        public override Vector2 GetAxis2D(Axis2D a)
        {
            return OVRInput.Get(ConvertAxis2D(a), MyController);
        }
    }
    #endregion
    #region Leaderboards
    public abstract class LeaderboardApi
    {
        public const int NumLBEntries = 15;

        private Dictionary<string, HSFloatInt> localScores = new Dictionary<string, HSFloatInt>();
        private Dictionary<string, int> localRanks = new Dictionary<string, int>();

        public bool HasHighScore(Modes mode, Guns gun)
        {
            string key = mode.ToString() + gun.ToString();
            return localScores.ContainsKey(key) || PlayerPrefs.HasKey(key);
        }
        public float GetMyHighScore(Modes mode, Guns gun)
        {
            string key = mode.ToString() + gun.ToString();
            if (localScores.ContainsKey(key))
            {
                var h = localScores[key];
                PlayerPrefs.SetInt(key, h.RawVal);
                return h.DisplayFloat;
            }
            else
            {
                if (PlayerPrefs.HasKey(key))
                {
                    int i = PlayerPrefs.GetInt(key);
                    localScores[key] = new HSFloatInt(i);
                    return localScores[key].DisplayFloat;
                }
                else if (mode == Modes.Quickdraw)
                    return 9999f;
            }
            return 0;
        }
        protected void SetLocalScore(Modes mode, Guns gun, HSFloatInt newScore)
        {
            string key = mode.ToString() + gun.ToString();
            localScores[key] = newScore;
            PlayerPrefs.SetInt(key, newScore.RawVal);
        }
        protected void ClearLocalScore(Modes mode, Guns gun)
        {
            string key = mode.ToString() + gun.ToString();
            if (localScores.ContainsKey(key))
            {
                localScores.Remove(key);
            }
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        public int GetMyRank(Modes mode, Guns gun)
        {
            string key = mode.ToString() + gun.ToString();
            if (!localRanks.ContainsKey(key))
            {
                return 1;
            }
            return localRanks[key];
        }
        protected void SetLocalRank(Modes mode, Guns gun, int newRank)
        {
            string key = mode.ToString() + gun.ToString();
            localRanks[key] = newRank;
        }

        public abstract void GetHighScores(Modes mode, Guns gun, LeaderboardFilter filter, AsyncApiDelegate<Score[]> Callback);

        public bool TrySetHighScore(Modes mode, Guns gun, float newHS)
        {
            //Reject any scores we know are impossibly high
            if (newHS > 50000f)
                return false;

            float curHs = GetMyHighScore(mode, gun);

            bool b = false;

            if (mode == Modes.Quickdraw)
            {
                if (newHS < curHs)
                {
                    b = true;
                }
            }
            else
            {
                if (newHS > curHs)
                {
                    b = true;
                }
            }

            if (b)
            {
                HSFloatInt h = new HSFloatInt(newHS);
                SetLocalScore(mode, gun, h);
                OnHighScoreSet(mode, gun, h);
            }

            CheckTopThatAchievementProgress();

            return b;
        }
        protected abstract void OnHighScoreSet(Modes mode, Guns gun, HSFloatInt newHS);

        private void CheckTopThatAchievementProgress()
        {
            Guns[] guns = new Guns[] { Guns.OverUnder, Guns.SideBySide, Guns.PumpAction, Guns.Tommygun };
            Modes[] modes = new Modes[] { Modes.AnimalGallery, Modes.Bounce, Modes.Targetwall, Modes.Trap, Modes.Quickdraw, Modes.Skeet };
            bool success = true;
            for (int g = 0; g < guns.Length; g++)
            {
                for (int m = 0; m < modes.Length; m++)
                {
                    if (!HasHighScore(modes[m], guns[g]))
                        success = false;
                }
            }
            if (success)
                VRApiManager.Achievements.GrantAchievement(Achievements.TOP_THAT);

        }
    }
    public class SteamLeaderboard : LeaderboardApi
    {
        private Dictionary<string, SteamLeaderboard_t> LeaderboardHandles;
        private MultiCR<LeaderboardScoresDownloaded_t> m_DLLBResults;
        private MultiCR<LeaderboardFindResult_t> m_FindLBResults;
        private MultiCR<LeaderboardScoreUploaded_t> m_ULLBScore;

        private int inits = 0;
        private int inito = 0;

        public SteamLeaderboard()
        {
            m_DLLBResults = new MultiCR<LeaderboardScoresDownloaded_t>();
            m_FindLBResults = new MultiCR<LeaderboardFindResult_t>();
            m_ULLBScore = new MultiCR<LeaderboardScoreUploaded_t>();
        }

        public void Initialize()
        {
            if (SteamManager.Initialized)
            {
                LeaderboardHandles = new Dictionary<string, SteamLeaderboard_t>();

                inits = 0;
                Init_Modes(Modes.Bounce);
                Init_Modes(Modes.Quickdraw);
                Init_Modes(Modes.Skeet);
                Init_Modes(Modes.Targetwall);
                Init_Modes(Modes.Trap);
                Init_Modes(Modes.AnimalGallery);
            }
        }

        private void Init_Modes(Modes mode)
        {
            Init_Guns(mode, Guns.OverUnder);
            Init_Guns(mode, Guns.PumpAction);
            Init_Guns(mode, Guns.SideBySide);
            Init_Guns(mode, Guns.Tommygun);
        }
        private void Init_Guns(Modes mode, Guns gun)
        {
            string s = LeaderboardKeyFromEnums(mode, gun);
            var apiToken = SteamUserStats.FindLeaderboard(s);

            inits++;

            m_FindLBResults.Set(apiToken, (g, IOFail) =>
            {
                inito++;

                if (!IOFail)
                {
                    LeaderboardHandles[s] = g.m_hSteamLeaderboard;
                }
                else
                {
                    Debug.LogError("Error initializing leaderboard: " + s);
                }

                InitsCheck();
            });
        }
        private void InitsCheck()
        {
            const string CheckedKey = "UpdatedPastHS";

            if (inito != inits)
            {
                return;
            }

            Debug.Log("Finished initializing steam leaderboards... ");
            inito = 0;
            inits = 0;

            if (PlayerPrefs.HasKey(CheckedKey))
            {
                Debug.Log("Caching local user high scores from steam... ");

                Guns[] gEnums = new Guns[] { Guns.OverUnder, Guns.PumpAction, Guns.SideBySide, Guns.Tommygun };
                Modes[] mEnums = new Modes[] { Modes.AnimalGallery, Modes.Bounce, Modes.Quickdraw, Modes.Skeet, Modes.Targetwall, Modes.Trap };

                var id = new CSteamID[] { SteamUser.GetSteamID() };
                foreach (Guns g in gEnums)
                {
                    foreach (Modes m in mEnums)
                    {
                        string key = LeaderboardKeyFromEnums(m, g);
                        if (!LeaderboardHandles.ContainsKey(key))
                        {
                            Debug.LogError("Leaderboard Handle not found: " + key);
                            continue;
                        }
                        SteamLeaderboard_t lbHandle = LeaderboardHandles[key];
                        var api = SteamUserStats.DownloadLeaderboardEntriesForUsers(lbHandle, id, 1);
                        m_DLLBResults.Set(api, (dl, IOFail) =>
                        {
                            if (!IOFail)
                            {
                                var dlkey = dl.m_hSteamLeaderboardEntries;
                                LeaderboardEntry_t lbe;
                                int[] details = new int[1];
                                if (dl.m_cEntryCount == 1 && SteamUserStats.GetDownloadedLeaderboardEntry(dlkey, 0, out lbe, details, 1))
                                {
                                    var hsfi = new HSFloatInt(lbe.m_nScore);
                                    SetLocalScore(m, g, hsfi);
                                    SetLocalRank(m, g, lbe.m_nGlobalRank);
                                    DLCUnlock dlc = (DLCUnlock)details[0];
                                    if (dlc != VRApiManager.SteamDLC.UnlockedDLC())
                                    {
                                        UploadScoreToSteam(m, g, hsfi, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
                                    }
                                }
                                else
                                {
                                    ClearLocalScore(m, g);
                                }
                            }
                        });
                    }
                }
            }
            else
            {
                Debug.Log("Uploading local scores to steam for 1-time sync... ");
                //Update steam scores from local
                Guns[] gEnums = new Guns[] { Guns.OverUnder, Guns.SideBySide, Guns.PumpAction };
                Modes[] mEnums = new Modes[] { Modes.Trap, Modes.Quickdraw, Modes.Skeet };
                string[] guns = new string[] { "Over Under", "Side by Side", "Pump Action" };
                string[] modes = new string[] { "Trap", "Quickdraw", "Skeet" };

                for (int g = 0; g < guns.Length; g++)
                {
                    for (int m = 0; m < modes.Length; m++)
                    {
                        string key = guns[g] + modes[m];
                        if (PlayerPrefs.HasKey(key))
                        {
                            float f;
                            if (mEnums[m] == Modes.Quickdraw)
                            {
                                f = PlayerPrefs.GetFloat(key);
                            }
                            else
                            {
                                f = (float)PlayerPrefs.GetInt(key);
                            }

                            if (TrySetHighScore(mEnums[m], gEnums[g], f))
                            {
                                Debug.Log("Setting saved highscore for: " + guns[g] + modes[m] + " [" + f + "]");
                            }
                            PlayerPrefs.DeleteKey(key);
                        }
                    }
                }

                PlayerPrefs.SetString(CheckedKey, "true");
            }
        }

        public override void GetHighScores(Modes mode, Guns gun, LeaderboardFilter filter, AsyncApiDelegate<Score[]> Callback)
        {
            AsyncApiCallback<Score[]> token = new AsyncApiCallback<Score[]>();
            token.Status = Status.NotInitialized;

            if (SteamManager.Initialized)
            {
                token.Status = Status.ApiFail;

                string lbkey = LeaderboardKeyFromEnums(mode, gun);
                if (!LeaderboardHandles.ContainsKey(lbkey))
                {
                    Callback(token);
                    return;
                }

                SteamLeaderboard_t lb = LeaderboardHandles[lbkey];
                ELeaderboardDataRequest lbFilter = GetLBFilter(filter);
                var api = SteamUserStats.DownloadLeaderboardEntries(lb, lbFilter, 1, NumLBEntries);

                m_DLLBResults.Set(api, (dl, IOFail) =>
                {
                    if (IOFail)
                    {
                        Callback(token);
                        return;
                    }
                    else
                    {
                        var key = dl.m_hSteamLeaderboardEntries;
                        int count = dl.m_cEntryCount;

                        List<Score> scores = new List<Score>();

                        for (int i = 0; i < count && i < NumLBEntries; i++)
                        {
                            Score s = new Score();
                            LeaderboardEntry_t lbe;
                            int[] details = new int[1];

                            if (SteamUserStats.GetDownloadedLeaderboardEntry(key, i, out lbe, details, 1))
                            {
                                s.Name = SteamFriends.GetFriendPersonaName(lbe.m_steamIDUser);
                                s.Rank = lbe.m_nGlobalRank;
                                s.Value = new HSFloatInt(lbe.m_nScore).DisplayFloat;
                                s.DLCUnlock = (DLCUnlock)details[0];
                            }
                            scores.Add(s);
                        }

                        if (scores.Count == NumLBEntries)
                        {
                            int rank = GetMyRank(mode, gun);
                            if (rank > NumLBEntries)
                            {
                                Score s = new Score();
                                s.Name = SteamFriends.GetPersonaName();
                                s.Rank = rank;
                                s.Value = GetMyHighScore(mode, gun);
                                s.DLCUnlock = VRApiManager.SteamDLC.UnlockedDLC();
                                scores[NumLBEntries - 1] = s;
                            }
                        }

                        token.Payload = scores.ToArray();
                        token.Status = Status.Succeess;
                        Callback(token);
                    }
                });
            }
            else
            {
                Callback(token);
            }
        }
        protected override void OnHighScoreSet(Modes mode, Guns gun, HSFloatInt newHS)
        {
            UploadScoreToSteam(mode, gun, newHS, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest);
        }
        private void UploadScoreToSteam(Modes mode, Guns gun, HSFloatInt newHS, ELeaderboardUploadScoreMethod uploadFilter)
        {
            if (SteamManager.Initialized)
            {
                string key = LeaderboardKeyFromEnums(mode, gun);
                if (!LeaderboardHandles.ContainsKey(key))
                {
                    Debug.LogError("Uninitialized Leaderboard: " + key);
                    return;
                }

                DLCUnlock dlc = VRApiManager.SteamDLC.UnlockedDLC();
                var api = SteamUserStats.UploadLeaderboardScore(LeaderboardHandles[key], uploadFilter, newHS.RawVal, new int[] { (int)dlc }, 1);
                m_ULLBScore.Set(api, (lbt, IOFail) =>
                {
                    if (IOFail || lbt.m_bSuccess == 0)
                    {
                        Debug.LogError("Error posting score to steam");
                        return;
                    }
                    SetLocalRank(mode, gun, lbt.m_nGlobalRankNew);
                });
            }
        }
        private ELeaderboardDataRequest GetLBFilter(LeaderboardFilter lbf)
        {
            if (lbf == LeaderboardFilter.Friends)
                return ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends;
            return ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal;
        }

        public string LeaderboardKeyFromEnums(Modes mode, Guns gun)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (mode == Modes.Quickdraw)
            {
                sb.Append(VRApiManager.ControlsApi.ToString());
            }
            sb.Append(mode.ToString());
            sb.Append(gun.ToString());
            return sb.ToString();
        }
    }
    public class OculusLeaderboard : LeaderboardApi
    {
        private User OculusUser;

        public void Initialize()
        {
            if (Core.IsInitialized())
            {
                var g = Users.GetLoggedInUser();
                g.OnComplete((x) =>
                {
                    if (x.IsError)
                    {
                        Error e = x.GetError();
                        Debug.Log("Users.GetLoggedInUser() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);
                        return;
                    }
                    OculusUser = x.Data;
                    Init_Modes(Modes.Bounce);
                    Init_Modes(Modes.Quickdraw);
                    Init_Modes(Modes.Skeet);
                    Init_Modes(Modes.Targetwall);
                    Init_Modes(Modes.Trap);
                    Init_Modes(Modes.AnimalGallery);
                });
            }
        }

        private void Init_Modes(Modes mode)
        {
            Init_Guns(mode, Guns.OverUnder);
            Init_Guns(mode, Guns.PumpAction);
            Init_Guns(mode, Guns.SideBySide);
            Init_Guns(mode, Guns.Tommygun);
        }
        private void Init_Guns(Modes mode, Guns gun)
        {
            string s = LeaderboardKeyFromEnums(mode, gun);

            var request = Oculus.Platform.Leaderboards.GetEntries(s, 1, LeaderboardFilterType.Unknown, LeaderboardStartAt.CenteredOnViewer);

            request.OnComplete((x) =>
            {
                if (x.IsError)
                {
                    Error e = x.GetError();
                    Debug.Log("Leaderboards.GetEntries() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);
                    return;
                }

                if (x.Data.Count != 1)
                {
                    Debug.Log("Invalid leaderboard cache [oculus]");
                    return;
                }

                if (x.Data[0].User.ID == OculusUser.ID)
                {
                    int score = (int)x.Data[0].Score;
                    int rank = x.Data[0].Rank;
                    SetLocalScore(mode, gun, new HSFloatInt(score));
                    SetLocalRank(mode, gun, rank);
                    DLCUnlock curDlc = VRApiManager.OculusDLC.UnlockedDLC();
                    DLCUnlock savedDLC = (DLCUnlock)x.Data[0].ExtraData[0];
                    if (curDlc > savedDLC)
                    {
                        SetHighScore(mode, gun, score, true);
                    }
                }
                else
                {
                    ClearLocalScore(mode, gun);
                }
            });
        }

        public string LeaderboardKeyFromEnums(Modes mode, Guns gun)
        {
            return mode.ToString() + gun.ToString();
        }

        public override void GetHighScores(Modes mode, Guns gun, LeaderboardFilter filter, AsyncApiDelegate<Score[]> Callback)
        {
            AsyncApiCallback<Score[]> token = new AsyncApiCallback<Score[]>();
            token.Status = Status.NotInitialized;

            if (!Core.IsInitialized())
            {
                Debug.Log("GettingHighScores() failed, Oculus API not initialized");
                Callback(token);
                return;
            }

            string key = LeaderboardKeyFromEnums(mode, gun);
            var request = Leaderboards.GetEntries(key, NumLBEntries, GetFilter(filter), LeaderboardStartAt.Top);
            //Debug.Log(string.Format("Requesting Leaderboards.GetEntries({0}, {1}, {2})", key, NumLBEntries, GetFilter(filter)));
            request.OnComplete((x) =>
            {
                if (x.IsError)
                {
                    token.Status = Status.ApiFail;
                    Error e = x.GetError();
                    Debug.Log("Leaderboards.GetEntries() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);
                }
                else
                {
                    token.Status = Status.Succeess;
                    var list = x.Data;
                    //Debug.Log("\tEntries found: " + list.Count);
                    var scores = new List<Score>();
                    for (int i = 0; i < list.Count && i < NumLBEntries; i++)
                    {
                        Score s = new Score();
                        s.Name = list[i].User.OculusID;
                        s.Rank = list[i].Rank;
                        s.Value = new HSFloatInt((int)list[i].Score).DisplayFloat;
                        s.DLCUnlock = (DLCUnlock)list[i].ExtraData[0];
                        scores.Add(s);
                    }

                    if (scores.Count == NumLBEntries)
                    {
                        int rank = GetMyRank(mode, gun);
                        if (rank > NumLBEntries)
                        {
                            Score s = new Score();
                            s.Name = SteamFriends.GetPersonaName();
                            s.Rank = rank;
                            s.Value = GetMyHighScore(mode, gun);
                            s.DLCUnlock = VRApiManager.SteamDLC.UnlockedDLC();
                            scores[NumLBEntries - 1] = s;
                        }
                    }

                    token.Payload = scores.ToArray();
                    //Debug.Log("High scores updated: " + scores.Count + " total");
                }
                Callback(token);
            });
        }
        private LeaderboardFilterType GetFilter(LeaderboardFilter filter)
        {
            if (filter == LeaderboardFilter.Friends)
                return LeaderboardFilterType.Friends;
            else if (filter == LeaderboardFilter.Global)
                return LeaderboardFilterType.None;
            return LeaderboardFilterType.Unknown;
        }
        protected override void OnHighScoreSet(Modes mode, Guns gun, HSFloatInt newHS)
        {
            SetHighScore(mode, gun, newHS.RawVal, false);
        }
        private void SetHighScore(Modes m, Guns g, long hs, bool force)
        {
            string key = LeaderboardKeyFromEnums(m, g);
            byte[] data = new byte[] { (byte)VRApiManager.OculusDLC.UnlockedDLC() };
            var req = Leaderboards.WriteEntry(key, hs, data, force);
            req.OnComplete((x) =>
            {
                if (x.IsError)
                {
                    Error e = x.GetError();
                    Debug.Log("Leaderboards.GetEntries() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);
                }
                else
                {
                    //Debug.Log("Leaderboard successfully updated");
                }
            });
        }
    }

    public class Score
    {
        public string Name;
        public int Rank;
        public float Value;
        public DLCUnlock DLCUnlock;

        public Score()
        { }
    }
    #endregion
    #region DLC
    public abstract class DLCApi
    {
        public abstract DLCUnlock UnlockedDLC();
    }
    public class SteamDLC : DLCApi
    {
        private AppId_t Bronze;
        private AppId_t Silver;
        private AppId_t Gold;

        public SteamDLC()
        {
            Bronze = new AppId_t(567420);
            Silver = new AppId_t(567421);
            Gold = new AppId_t(567422);
        }

        public override DLCUnlock UnlockedDLC()
        {
            var dlc = DLCUnlock.None;
            if (SteamManager.Initialized)
            {
                if (SteamApps.BIsDlcInstalled(Bronze))
                {
                    dlc |= DLCUnlock.Bronze;
                }
                if (SteamApps.BIsDlcInstalled(Silver))
                {
                    dlc |= DLCUnlock.Silver;
                }
                if (SteamApps.BIsDlcInstalled(Gold))
                {
                    dlc |= DLCUnlock.Gold;
                }
            }
            return dlc;
        }
    }
    public class OculusDLC : DLCApi
    {
        public const string BronzeSKU = "Bronze";
        public const string SilverSKU = "Silver";
        public const string GoldSKU = "Gold";

        private DLCUnlock cache = DLCUnlock.None;

        public OculusDLC() { }

        public void UpdateCache()
        {
            UpdateCache(null);
        }
        public void UpdateCache(Action OnComplete)
        {
            var g = IAP.GetViewerPurchases();
            g.OnComplete((x) =>
            {
                if (!x.IsError)
                {
                    cache = DLCUnlock.None;
                    for (int i = 0; i < x.Data.Count; i++)
                    {
                        if (x.Data[i].Sku == BronzeSKU)
                        {
                            cache |= DLCUnlock.Bronze;
                        }
                        else if (x.Data[i].Sku == SilverSKU)
                        {
                            cache |= DLCUnlock.Silver;
                        }
                        else if (x.Data[i].Sku == GoldSKU)
                        {
                            cache |= DLCUnlock.Gold;
                        }
                    }
                    Debug.Log("Owned DLC: " + cache.ToString());
                }
                else
                {
                    Error e = x.GetError();
                    Debug.Log("IAP.GetViewerPurchases() Error code: " + e.Code + " http: " + e.HttpCode + " msg: " + e.Message);
                }

                if (OnComplete != null)
                    OnComplete();
            });
        }

        public override DLCUnlock UnlockedDLC()
        {
            return cache;
        }
        public string SkuFromDLC(DLCUnlock dlc)
        {
            switch (dlc)
            {
                case DLCUnlock.Bronze:
                    return BronzeSKU;
                case DLCUnlock.Silver:
                    return SilverSKU;
                case DLCUnlock.Gold:
                    return GoldSKU;
            }
            return "Invalid dlc";
        }
    }
    #endregion
}