using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;

    void Awake()
    {
        //若其他manager存在，那就不需要自己
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        //若沒有其他Manager存在，則保留自己
        DontDestroyOnLoad(gameObject);
        instance = this;

    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadScene)
    {
        Debug.Log("Photon test scene " + scene.name + " " + (scene.name == "Character selection_Online"));
        if (scene.name == "Character selection_Online") //in custom scene
        {
            //Add player to list
            Debug.Log("Player count " + PhotonNetwork.CountOfPlayers);

            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                Debug.Log("Phonton player " + i.ToString());
                LocalRoomManager.instance.AddOnlinePlayer(PhotonNetwork.PlayerList[i], i);
            }
        }
        else if (scene.name == "Menu") {
            LocalRoomManager.instance.ClearPlayerDatas();
            PhotonNetwork.Disconnect();
            Destroy(gameObject);
        }
    }
}
