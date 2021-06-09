using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalRoomManager : MonoBehaviourPunCallbacks
{
    public static LocalRoomManager instance;
    public event Action<LocalPlayerProperty> OnLocalPlayerAdded;
    public event Action<LocalPlayerProperty> OnOnlinePlayerAdded;
    public List<LocalPlayerProperty> players = new List<LocalPlayerProperty>();
    public LocalPlayerProperty gamePlaySetting = new LocalPlayerProperty();
    public event Action<Dictionary<string, object>> OnPlayerValueChanged;
    Dictionary<int, List<int>> team_player_dict = new Dictionary<int, List<int>>(); //team code , player_index


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
        //AddLocalPlayer();  //Add first

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
        Debug.Log("player i  count  " + players.Count);

        //add slot 
        OnLocalPlayerAdded?.Invoke(_local);
    }
    public void AddOnlinePlayer(Player _player, int _index)
    {
        LocalPlayerProperty _local = new LocalPlayerProperty();
        _local.SetProperty("Player", _player);
        _local.SetProperty("PlayerIndex", _index);
        players.Add(_local);
        Debug.Log("Photon Add Player " + _player.NickName);
        OnOnlinePlayerAdded?.Invoke(_local);
    }

    public virtual void StartGamePlay()
    {
        //set up data and change Scene
        SceneManager.LoadScene((string)gamePlaySetting.playerProperty[GameplaySettingControl.MAP_OPT]);
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


                //Log team member
                if (team_player_dict.ContainsKey((int)players[i].playerProperty[CustomPropertyCode.TEAM_CODE]))
                {
                    team_player_dict[(int)players[i].playerProperty[CustomPropertyCode.TEAM_CODE]].Add(i);
                }
                else
                {
                    team_player_dict.Add((int)players[i].playerProperty[CustomPropertyCode.TEAM_CODE], new List<int>());
                    team_player_dict[(int)players[i].playerProperty[CustomPropertyCode.TEAM_CODE]].Add(i);
                }


            }


        }
    }

    public PlayerControl Revive(int _playerData_index)
    {
        PlayerControl _player = Instantiate(Resources.Load("Prefab/Player") as GameObject, Vector2.zero, Quaternion.identity).GetComponent<PlayerControl>();
        Debug.Log("player i " + _playerData_index);
        _player.transform.position = MapControl.instance.viewWorldCenter;
        _player.SetUp(players[_playerData_index], _playerData_index);

        _player.AddRevive();
        return _player;
    }

    public int[] Get_Index_In_Same_Team(int teamCode)
    {
        return team_player_dict[teamCode].ToArray();
    }

    public int[] Get_Index_In_Different_Team(int teamCode)
    {
        List<int> _res = new List<int>();
        foreach (KeyValuePair<int, List<int>> pk in team_player_dict)
        {
            if (pk.Key != teamCode)
            {
                _res.AddRange(pk.Value);
            }
        }

        return _res.ToArray();
    }

    public void ClearPlayerDatas()
    {
        players.Clear();
        gamePlaySetting.playerProperty.Clear();
    }
}
