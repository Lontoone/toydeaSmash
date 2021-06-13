using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPun, IPunOwnershipCallbacks
{
    PhotonView pv;
    GameObject controller;

    public void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void SwitchOwnerShip(Player _owner)
    {
        if (_owner == PhotonNetwork.LocalPlayer)
        {
            pv.RequestOwnership();
            controller.GetComponent<PhotonView>().RequestOwnership();
        }
    }
    public void CreateController(Player _owner)
    {
        Debug.Log("Instantiated Player Controller");
        //controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector2.zero, Quaternion.identity, 0, new object[] { pv.ViewID });
        controller = PhotonNetwork.Instantiate("Prefab/Player_Variant_Online", Vector2.zero, Quaternion.identity);
        //controller.GetComponent<PlayerControl>().SetUpOnline();
        SwitchOwnerShip(_owner);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        //CreateController();
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        base.photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        //throw new System.NotImplementedException();
    }
}
