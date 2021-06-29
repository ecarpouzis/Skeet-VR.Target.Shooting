using UnityEngine;
using System.Collections;

public class CreditsControls : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        bool b = false;
        if (VRApiManager.Controls.IsControllerActive(Controller.Left))
        {
            b |= VRApiManager.Controls.Left.GetButtonDown(Buttons.CallPull);
            b |= VRApiManager.Controls.Left.GetButtonDown(Buttons.FireGun);
            b |= VRApiManager.Controls.Left.GetButtonDown(Buttons.ReleaseEmptyAmmo);
            b |= VRApiManager.Controls.Left.GetButtonDown(Buttons.SpawnAmmo);
            b |= VRApiManager.Controls.Left.GetButtonDown(Buttons.Teleport);
            b |= VRApiManager.Controls.Left.GetButtonDown(Buttons.ToggleControls);
        }
        if (VRApiManager.Controls.IsControllerActive(Controller.Right))
        {
            b |= VRApiManager.Controls.Right.GetButtonDown(Buttons.CallPull);
            b |= VRApiManager.Controls.Right.GetButtonDown(Buttons.FireGun);
            b |= VRApiManager.Controls.Right.GetButtonDown(Buttons.ReleaseEmptyAmmo);
            b |= VRApiManager.Controls.Right.GetButtonDown(Buttons.SpawnAmmo);
            b |= VRApiManager.Controls.Right.GetButtonDown(Buttons.Teleport);
            b |= VRApiManager.Controls.Right.GetButtonDown(Buttons.ToggleControls);
        }

        if (b)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Trap");
        }
    }
}
