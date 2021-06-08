using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultSceneControl : MonoBehaviour
{
    //public PlayerResultPanel scorePanel_prefab;
    //public GameObject 
    public GameObject container;
    public void Start()
    {

        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            //PlayerResultPanel _p = Instantiate(scorePanel_prefab, Vector3.zero, Quaternion.identity, container.transform);

            //_p.SetUp(i);
        }
    }

    public void Back() {
        //TODO: back to room 

        //temp back to start:
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
        //temp : reset localplayer data
        LocalRoomManager.instance.ClearPlayerDatas();
        Destroy(LocalRoomManager.instance.gameObject);
    }
}
