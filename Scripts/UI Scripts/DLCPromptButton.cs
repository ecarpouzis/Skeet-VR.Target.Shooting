using UnityEngine;
using System.Collections;

public class DLCPromptButton : Shootable
{

    override public void OnHit()
    {
        DLCStore.Instance.ToggleDLC();
    }
}
