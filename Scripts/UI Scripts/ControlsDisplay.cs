using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControlsDisplay : MonoBehaviour
{
    public const string False = "false";
    public const string True = "true";
    public const string PlayerPrefsControlsKey = "SaveControlsToggle";

    public bool controlsActive = true;
    public GameObject leftControlsCanvas;
    public GameObject rightControlsCanvas;
    public GameObject rightControllerModel;
    GameObject quickdrawCanvas;
    public Text quickloadText;
    public GameObject TeleportText;


    public Text LeftTrigger;
    public Text LeftGrip;
    public Text RightTouchpad;
    public Text LeftTouchpad;
    public Text LeftTouchpadAlt;

    private bool silenceFirstModel = false;

    public void LeftTouchpadText(bool isTeleport)
    {
        if (isTeleport)
        {
            LeftTouchpad.gameObject.SetActive(true);
            LeftTouchpadAlt.gameObject.SetActive(false);
        }
        else
        {
            LeftTouchpad.gameObject.SetActive(false);
            LeftTouchpadAlt.gameObject.SetActive(true);
        }
    }

    public void SetUpQuickdrawUI()
    {
        quickdrawCanvas = GameObject.Find("QuickdrawCanvas");
        GameObject foundText = GameObject.Find("QuickdrawText");
        if (foundText != null)
        {
            quickloadText = foundText.GetComponent<Text>();
        }
    }

    void Start()
    {
        silenceFirstModel = false;

        bool b = !PlayerPrefs.HasKey(PlayerPrefsControlsKey) || PlayerPrefs.GetString(PlayerPrefsControlsKey) == True;
        if (b)
        {
            ShowControls(true);
        }
        else
        {
            ShowControls(false);
            silenceFirstModel = true;
        }
        rightControllerModel.SetActive(true);
    }

    void Update()
    {
        if (PlayerController.Instance.CanTeleport)
        {
            TeleportText.SetActive(true);
        }
        else
        {
            TeleportText.SetActive(false);
        }

        if (silenceFirstModel)
        {
            if (rightControllerModel.transform.childCount > 0)
            {
                rightControllerModel.SetActive(false);
                silenceFirstModel = false;
            }
        }
    }

    public void EnableQuickloadText(bool setActive)
    {
        quickloadText.enabled = setActive;
    }

    public void SetSBSControls()
    {
        if (VRApiManager.c_IsOculus)
        {
            RightTouchpad.text = "(A) Open\nBarrel";
        }
        else
        {
            RightTouchpad.text = "Open\nBarrel";
        }
        LeftTrigger.text = "Spawn\nAmmo";
    }

    public void SetOUControls()
    {
        if (VRApiManager.c_IsOculus)
        {
            RightTouchpad.text = "(A) Open\nBarrel";
        }
        else
        {
            RightTouchpad.text = "Open\nBarrel";
        }
        LeftTrigger.text = "Spawn\nAmmo";
    }

    public void SetTommygunControls()
    {
        RightTouchpad.text = "";
        LeftTrigger.text = "";
    }

    public void SetPumpControls()
    {
        RightTouchpad.text = "";
        LeftTrigger.text = "Grab\nPump";
    }

    public void ShowControls(bool show)
    {
        silenceFirstModel = false;
        if (show)
        {
            leftControlsCanvas.SetActive(true);
            rightControlsCanvas.SetActive(true);
            rightControllerModel.SetActive(true);
            controlsActive = true;
            PlayerPrefs.SetString(PlayerPrefsControlsKey, True);
        }
        else
        {
            leftControlsCanvas.SetActive(false);
            rightControlsCanvas.SetActive(false);
            controlsActive = false;
            PlayerPrefs.SetString(PlayerPrefsControlsKey, False);
            rightControllerModel.SetActive(false);
        }
    }
}
