using UnityEngine;
using System.Collections;

public class ShellEject : MonoBehaviour
{
    public GameObject shellPrefab;

    public void ejectShell()
    {
        GameObject shell = Instantiate(shellPrefab, transform.position, transform.rotation) as GameObject;
        shell.GetComponent<Rigidbody>().AddForce(transform.right * 100f);
    }
}
