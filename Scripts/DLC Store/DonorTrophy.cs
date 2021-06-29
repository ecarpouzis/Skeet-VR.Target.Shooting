using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonorTrophy : MonoBehaviour
{

    public MeshRenderer TrophyRenderer;

    public Material Bronze;
    public Material Silver;
    public Material Gold;

    public GameObject BronzeButton;
    public GameObject SilverButton;
    public GameObject GoldButton;

    private float WaitForSteam = 0f;

    // Use this for initialization
    private void Awake()
    {
        WaitForSteam = 0f;

        BronzeButton.SetActive(false);
        SilverButton.SetActive(false);
        GoldButton.SetActive(false);
        TrophyRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (WaitForSteam < 5f)
        {
            WaitForSteam += Time.deltaTime;
            if (VRApiManager.p_IsInit)
            {
                SetDLC(VRApiManager.DLC.UnlockedDLC());
                WaitForSteam = 10f;
            }
        }
    }

    private void SetDLC(DLCUnlock dlc)
    {
        if (dlc == DLCUnlock.None)
        {
            return;
        }

        TrophyRenderer.gameObject.SetActive(true);

        int i = (int)dlc;
        bool hasMultiple = (i != 0) && (i & (i - 1)) != 0;
        if (dlc.HasDLC(DLCUnlock.Bronze))
        {
            SetMaterial(Bronze);
            if (hasMultiple)
            {
                BronzeButton.SetActive(true);
            }
        }
        if (dlc.HasDLC(DLCUnlock.Silver))
        {
            SetMaterial(Silver);
            if (hasMultiple)
            {
                SilverButton.SetActive(true);
            }
        }
        if (dlc.HasDLC(DLCUnlock.Gold))
        {
            SetMaterial(Gold);
            if (hasMultiple)
            {
                GoldButton.SetActive(true);
            }
        }
    }
    private void SetMaterial(Material mat)
    {
        TrophyRenderer.material = mat;
    }
}
