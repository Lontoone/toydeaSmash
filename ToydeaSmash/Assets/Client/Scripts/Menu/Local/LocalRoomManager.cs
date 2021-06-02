using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LocalRoomManager : MonoBehaviour
{
    public static LocalRoomManager instance;
    public List<LocalPlayerProperty> players = new List<LocalPlayerProperty>();

    public event Action<LocalPlayerProperty> OnLocalPlayerAdded;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        DontDestroyOnLoad(this);
    }

    public IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        AddLocalPlayer();  //Add first

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    public void AddLocalPlayer()
    {
        LocalPlayerProperty _local = new LocalPlayerProperty();
        players.Add(_local);
        Debug.Log("player i  count  "+players.Count);
        //add slot 
        if (OnLocalPlayerAdded != null)
        {
            OnLocalPlayerAdded.Invoke(_local);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadScene)
    {
        //if in game play
        if (scene.name == "GamePlay")
        {
            //generate player
            Debug.Log(" Create player!");

            //for each playerData=> Gernerate local multiplayer with id
            for (int i = 0; i < players.Count; i++)
            {
                PlayerControl _player = Instantiate(Resources.Load("Prefab/Player") as GameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerControl>();
                Debug.Log("player i " + i);
                _player.transform.position = MapControl.instance.viewWorldCenter;
                _player.SetUp(players[i], i);

                _player.AddLanding();

            }


        }
    }

    public void Revive(int _playerData_index) {
        PlayerControl _player = Instantiate(Resources.Load("Prefab/Player") as GameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerControl>();
        Debug.Log("player i " + _playerData_index);
        _player.transform.position = MapControl.instance.viewWorldCenter;
        _player.SetUp(players[_playerData_index], _playerData_index);

        _player.AddRevive();
    }


}
