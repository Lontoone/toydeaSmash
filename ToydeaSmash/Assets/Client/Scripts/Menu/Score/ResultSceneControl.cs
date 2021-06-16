using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneControl : MonoBehaviour
{
    //public PlayerResultPanel scorePanel_prefab;
    //public GameObject 
    public GameObject container;
    public Transform[] spawnPoints;
    public PlayerResultPanel[] playerResultPanels;
    public void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            List<int> _placeSort = LocalRoomManager.instance.SortPlayerByKillAmount();
            int _playerIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.PLAYER_INDEX];
            int _playerPlace = (int)_placeSort.IndexOf(_playerIndex);

            GenerateOnlinePlayer(_playerIndex, _playerPlace);
        }
        else
        {
            GeneratePlayer();
        }
    }

    private void GeneratePlayer()
    {
        Debug.Log("result player count " + LocalRoomManager.instance.players.Count);
        //score
        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            playerResultPanels[i].gameObject.SetActive(true);
            playerResultPanels[i].SetUp(i);

            PlayerControl _player = Instantiate(Resources.Load("Prefab/Player") as GameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerControl>();
            _player.SetUp(
                LocalRoomManager.instance.players[i], i);
            _player.transform.position = spawnPoints[i].position;
        }
    }
    private void GenerateOnlinePlayer(int _playerIndex, int _place)
    {
        PlayerControl _player = LocalRoomManager.instance.InstantiateOnlinePlayer(_playerIndex);
        _player.transform.position = _player.transform.position = spawnPoints[_place].position;
    }

    public void Back()
    {
        //TODO: back to room 

        //temp back to start:
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        //temp : reset localplayer data
        LocalRoomManager.instance.ClearPlayerDatas();

        Destroy(LocalRoomManager.instance.gameObject);
        //GCManager.Clear();

    }
}
