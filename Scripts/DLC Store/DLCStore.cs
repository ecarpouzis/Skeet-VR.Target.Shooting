using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLCStore : MonoBehaviour
{
    public static DLCStore Instance;
    static bool showingDLC;
    public GameObject Scoreboard, DLCPromptCanvas, OVRDLCStore, OpenVRDLCStore, DLCExitCanvas;
    public GameObject BronzeUnpurchased, BronzePurchased, SilverUnpurchased, SilverPurchased, GoldUnpurchased, GoldPurchased;

    void Awake()
    {
        showingDLC = false;
        Instance = this;
    }

    public void Update()
    {
        DisablePurchasedIcons();
    }

    public void ToggleDLC()
    {
        if (!showingDLC)
        {
            Scoreboard.SetActive(false);
            DLCPromptCanvas.SetActive(false);
            DLCExitCanvas.SetActive(true);
            if (VRApiManager.p_IsOculus)
            {
                OVRDLCStore.SetActive(true);
                DisablePurchasedIcons();
            }
            else
            {
                OpenVRDLCStore.SetActive(true);
            }
            showingDLC = true;
        }
        else
        {
            Scoreboard.SetActive(true);
            DLCPromptCanvas.SetActive(true);
            DLCExitCanvas.SetActive(false);
            OVRDLCStore.SetActive(false);
            OpenVRDLCStore.SetActive(false);
            showingDLC = false;
        }
    }

    //Since the oculus purchases happen directly in client,
    //we have to manually prevent multiple purchases so we
    //disable the button and display a "thank you" text
    //for any purchased trophy
    private void DisablePurchasedIcons()
    {
        if (VRApiManager.p_IsOculus)
        {
            DLCUnlock dlc = VRApiManager.DLC.UnlockedDLC();

            bool hbrz = dlc.HasDLC(DLCUnlock.Bronze);
            BronzePurchased.SetActive(hbrz);
            BronzeUnpurchased.SetActive(!hbrz);

            bool hslv = dlc.HasDLC(DLCUnlock.Silver);
            SilverPurchased.SetActive(hslv);
            SilverUnpurchased.SetActive(!hslv);

            bool hgld = dlc.HasDLC(DLCUnlock.Gold);
            GoldPurchased.SetActive(hgld);
            GoldUnpurchased.SetActive(!hgld);
        }
    }
}