using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuControl : MonoBehaviour
{
    public GameObject panel;
    public void Quit()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");        
    }
    public void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(true);
        }
    }
}
