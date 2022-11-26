using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PicAChoose : MonoBehaviour
{
    public GameObject canv;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            canv.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            canv.SetActive(false);
        }
    }

    public void startPAC()
    {
        SceneManager.LoadScene(1);
    }
}
