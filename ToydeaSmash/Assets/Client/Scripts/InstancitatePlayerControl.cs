using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstancitatePlayerControl : MonoBehaviourPun
{
    public static InstancitatePlayerControl instance;
    public bool isOnline = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Result")
        {
            Destroy(gameObject);
        }
        else if (arg0.name.Contains("GamePlay"))
        {
            if (isOnline)
            {
                CreateOnlinePlayer();
            }
            else
            {
                CreateLocalPlayer();
            }
        }
        else if (arg0.name == "Menu")
        {
            Destroy(gameObject);
        }

    }
    private void CreateLocalPlayer()
    {
        //generate player
        Debug.Log(" Create player!");
        LocalRoomManager.instance.InstantiateLocalPlayer();
    }
    private void CreateOnlinePlayer()
    {
        //generate player
        Debug.Log(" Create player!");

        LocalRoomManager.instance.InstantiateOnlinePlayer(
            (int)PhotonNetwork.LocalPlayer.CustomProperties["PlayerIndex"]
            );

        //if (base.photonView.Owner == PhotonNetwork.LocalPlayer)
        /*
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                PlayerManager _pm = PhotonNetwork.Instantiate("Prefab/PlayerManager_Online", Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
                _pm.CreateController(PhotonNetwork.PlayerList[i]);
            }

        }*/
    }
}
