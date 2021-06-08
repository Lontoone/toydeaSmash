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
        GeneratePlayer();
    }

    private void GeneratePlayer()
    {
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

    public void Back()
    {
        //TODO: back to room 

        //temp back to start:
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        //temp : reset localplayer data
        LocalRoomManager.instance.ClearPlayerDatas();

        Destroy(LocalRoomManager.instance.gameObject);
        GCManager.Clear();

    }
}
