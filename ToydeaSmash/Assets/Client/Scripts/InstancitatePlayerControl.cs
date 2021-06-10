using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstancitatePlayerControl : MonoBehaviour
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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Result")
        {
            Destroy(gameObject);
        }
        else if (arg0.name == "GamePlay")
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
    }
}
