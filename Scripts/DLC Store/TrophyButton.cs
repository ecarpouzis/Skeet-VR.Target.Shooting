using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyButton : Shootable {

    public Material BronzeMaterial, SilverMaterial, GoldMaterial;
    GameObject Trophy;
    
    public void Awake()
    {
        Trophy = GameObject.Find("TrophyModel");
    }

    public override void OnHit()
    {
        switch (this.name)
        {
            case "BronzeButton":
                Trophy.GetComponent<Renderer>().material = BronzeMaterial;
                break;
            case "SilverButton":
                Trophy.GetComponent<Renderer>().material = SilverMaterial;
                break;
            case "GoldButton":
                Trophy.GetComponent<Renderer>().material = GoldMaterial;
                break;
        }
    }
}
