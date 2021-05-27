using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public void ChangeScene_local(string _name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_name);
    }

    public void ChangeScene_Photon(string _name) {
        PhotonNetwork.LoadLevel(_name);
    }

}
