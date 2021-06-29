// The SteamManager is designed to work with Steamworks.NET
// This file is released into the public domain.
// Where that dedication is not recognized you are granted a perpetual,
// irrevokable license to copy and modify this files as you see fit.
//
// Version: 1.0.2

using UnityEngine;
using System.Collections;
using Steamworks;

//
// The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
// It handles the basics of starting up and shutting down the SteamAPI for use.
//
public class SteamManager : MonoBehaviour
{
    public static bool Initialized
    {
        get; private set;
    }

    private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
    private static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText)
    {
        Debug.LogWarning("Steam Error: " + pchDebugText);
    }

    private void Awake()
    {
        if (VRApiManager.SteamManager != null)
        {
            Debug.Log("Multiple SteamManagers!");
            Destroy(gameObject);
        }

        // We want our SteamManager Instance to persist across scenes.
        DontDestroyOnLoad(gameObject);

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
        }

        try
        {
            // Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
            // remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
            // See the Valve documentation for more information: https://partner.steamgames.com/documentation/drm#FAQ
            if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
            {
                Application.Quit();
                return;
            }
        }
        catch (System.DllNotFoundException e)
        { // We catch this exception here, as it will be the first occurence of it.
            Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);

            Application.Quit();
            return;
        }

        // Initialize the SteamAPI, if Init() returns false this can happen for many reasons.
        // Some examples include:
        // Steam Client is not running.
        // Launching from outside of steam without a steam_appid.txt file in place.
        // Running under a different OS User or Access level (for example running "as administrator")
        // Valve's documentation for this is located here:
        // https://partner.steamgames.com/documentation/getting_started
        // https://partner.steamgames.com/documentation/example // Under: Common Build Problems
        // https://partner.steamgames.com/documentation/bootstrap_stats // At the very bottom

        // If you're running into Init issues try running DbgView prior to launching to get the internal output from Steam.
        // http://technet.microsoft.com/en-us/sysinternals/bb896647.aspx
        Initialized = SteamAPI.Init();
        if (!Initialized)
        {
            Debug.LogWarning("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
            return;
        }
        else
        {
            SteamUserStats.RequestCurrentStats();
            Debug.Log("Steam Initialized [AppId=" + SteamUtils.GetAppID() + "][NumAchievements=" + SteamUserStats.GetNumAchievements() + "][NumFriends=" + SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll) + "], welcome " + SteamFriends.GetPersonaName());
            SendMessage("OnSteamInitialize", SendMessageOptions.DontRequireReceiver);
        }
    }

    // This should only ever get called on first load and after an Assembly reload, You should never Disable the Steamworks Manager yourself.
    private void OnEnable()
    {
        if (!Initialized)
        {
            return;
        }

        if (m_SteamAPIWarningMessageHook == null)
        {
            // Set up our callback to recieve warning messages from Steam.
            // You must launch with "-debug_steamapi" in the launch args to recieve warnings.
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }
    }


    // OnApplicationQuit gets called too early to shutdown the SteamAPI.
    // Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
    // Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
    private void OnDestroy()
    {
        if (!Initialized)
        {
            return;
        }

        SteamAPI.Shutdown();
    }

    private void Update()
    {
        if (!Initialized)
        {
            return;
        }

        // Run Steam client callbacks
        SteamAPI.RunCallbacks();
    }
}
