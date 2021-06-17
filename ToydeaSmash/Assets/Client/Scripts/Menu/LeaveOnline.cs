using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveOnline : MonoBehaviour
{
    public void QuitOnline()
    {
        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.Disconnect();
            LocalRoomManager.instance.ClearPlayerDatas();
        }
       
    }
}
