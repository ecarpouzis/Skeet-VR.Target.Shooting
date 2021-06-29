using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkeetRoundDisplay : MonoBehaviour
{
    public static SkeetRoundDisplay Instance;

    public GameObject DisplayHighTower, DisplayLowTower, DisplayNextStation;
    public Graphic HighTowerImage, LowTowerImage, NextStationImage;
    public Graphic HighTowerText, LowTowerText, NextStationText;
    public Graphic HighTowerSecondText, LowTowerSecondText, NextStationSecondText;
    public Text TouchpadText1, TouchpadText2, TouchpadText3;

    public void SetRoundValue(int value)
    {
        switch (value)
        {
            case 0:
                DisplayNextStation.SetActive(true);
                DisplayHighTower.SetActive(false);
                DisplayLowTower.SetActive(false);
                break;
            case 1:
                DisplayNextStation.SetActive(false);
                DisplayHighTower.SetActive(true);
                DisplayLowTower.SetActive(false);
                break;
            case 2:
                DisplayNextStation.SetActive(false);
                DisplayHighTower.SetActive(false);
                DisplayLowTower.SetActive(true);
                break;
            case 3:
                DisplayNextStation.SetActive(false);
                DisplayHighTower.SetActive(true);
                DisplayLowTower.SetActive(true);
                break;
            default:
                DeactivateUI();
                Debug.LogWarning("Invalid round value");
                break;
        }
    }

    public void DeactivateUI()
    {
        DisplayNextStation.SetActive(false);
        DisplayHighTower.SetActive(false);
        DisplayLowTower.SetActive(false);
    }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(VRApiManager.c_IsOculus)
        {
            TouchpadText1.text = "(X Button)";
            TouchpadText2.text = "(X Button)";
            TouchpadText3.text = "(X Button)";
        }
        StartCoroutine(FadeGraphics(.6f, HighTowerImage, LowTowerImage, NextStationImage, HighTowerText, LowTowerText, NextStationText, HighTowerSecondText, LowTowerSecondText, NextStationSecondText));
    }

    IEnumerator FadeGraphics(float duration, params Graphic[] graphics)
    {
        while (true)
        {
            foreach (Graphic g in graphics)
            {
                g.CrossFadeAlpha(0f, duration, false);
            }
            yield return new WaitForSeconds(duration);
            foreach (Graphic g in graphics)
            {
                g.CrossFadeAlpha(1f, duration, false);
            }
            yield return new WaitForSeconds(duration);
        }
    }
}
