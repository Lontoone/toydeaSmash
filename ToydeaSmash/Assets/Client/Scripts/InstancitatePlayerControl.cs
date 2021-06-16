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
        int _playerIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.PLAYER_INDEX];
        StartCoroutine(CreatePlayerDelayCoro(_playerIndex));
        /*
        Debug.Log(" Create player!");
        PlayerControl _player = LocalRoomManager.instance.InstantiateOnlinePlayer(
            (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.PLAYER_INDEX]
            );
        _player.AddLanding();*/
    }
    private IEnumerator CreatePlayerDelayCoro(int _playerIndex)
    {
        WaitForSeconds _wait = new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.5f * _playerIndex);
        PlayerControl _player = LocalRoomManager.instance.InstantiateOnlinePlayer(_playerIndex);
        _player.AddLanding();
    }

}
