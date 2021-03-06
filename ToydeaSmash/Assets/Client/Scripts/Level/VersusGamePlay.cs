using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Versus Game mode
public class VersusGamePlay : MonoBehaviourPun
{
    //public const string ISLOSE = "ISLOSE";
    public const byte PUN_ONPLAYER_LIFESTOCK_CHANGE_EVENTCODE = 5;
    //public const string LIFESTOCK = "LIFESTOCK";
    public GameObject lifeStockUIContainer;
    public PlayerLifeStockControl lifeStockItem_prefab;

    public Dictionary<int, int> playerLifeStock = new Dictionary<int, int>(); //playerIndex, life stock
    private Dictionary<int, PlayerLifeStockControl> lifeStockUI = new Dictionary<int, PlayerLifeStockControl>();

    public void Awake()
    {
        PlayerControl.OnCreate += RegisterPlayer;
        PlayerControl.OnDestory += CheckPlayerRevive;

        HitableObj.OnKilled += CheckTeamWinLose;
        //PhotonNetwork.AddCallbackTarget(this);  // This is not working correctly
        PhotonNetwork.NetworkingClient.EventReceived += PUNOnLifeStockChangeEvent;
    }
    public void OnDestroy()
    {
        PlayerControl.OnCreate -= RegisterPlayer;
        PlayerControl.OnDestory -= CheckPlayerRevive;

        HitableObj.OnKilled -= CheckTeamWinLose;
        //PhotonNetwork.RemoveCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived -= PUNOnLifeStockChangeEvent;
    }


    //players first join the game
    public void RegisterPlayer(int _index)
    {
        if (!playerLifeStock.ContainsKey(_index))
        {
            LocalRoomManager.instance.players[_index].SetProperty(CustomPropertyCode.ISLOSE, false);
            LocalRoomManager.instance.players[_index].SetProperty(CustomPropertyCode.LIFESTOCK, LocalRoomManager.instance.gamePlaySetting.GetValue<int>(GameplaySettingControl.LIFESTOCK_OPT));
            //int _playerIndex = LocalRoomManager.instance.players[_index].GetValue<int>(CustomPropertyCode.PLAYER_INDEX);
            playerLifeStock.Add(_index, LocalRoomManager.instance.gamePlaySetting.GetValue<int>(GameplaySettingControl.LIFESTOCK_OPT));

            //base.photonView.RPC("SetLifeStockColor", RpcTarget.All,_index, _index);
            PunSendLifeStockChangeEvent(_index, LocalRoomManager.instance.gamePlaySetting.GetValue<int>(GameplaySettingControl.LIFESTOCK_OPT));

            //generate life stock ui item for each player
            PlayerLifeStockControl _ui = Instantiate(lifeStockItem_prefab, Vector3.zero, Quaternion.identity, lifeStockUIContainer.transform);
            lifeStockUI.Add(_index, _ui);
            SetLifeStockColor(_index);
            lifeStockUI[_index].SetLifeCount(LocalRoomManager.instance.players[_index].GetValue<int>(CustomPropertyCode.LIFESTOCK));
        }
    }

    private void PunSendLifeStockChangeEvent(int _index, int _lifeStock)
    {
        if (!PhotonNetwork.IsConnected)
        {
            return;
        }
        object[] content = new object[] { _index, _lifeStock };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(PUN_ONPLAYER_LIFESTOCK_CHANGE_EVENTCODE, content, raiseEventOptions, SendOptions.SendReliable);
    }
    private void PUNOnLifeStockChangeEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == PUN_ONPLAYER_LIFESTOCK_CHANGE_EVENTCODE)
        {
            object[] data = (object[])photonEvent.CustomData;

            lifeStockUI[(int)data[0]].SetLifeCount((int)data[1]);
            SetLifeStockColor((int)data[0]);
            Debug.Log("pun event life stock " + (int)data[1]);
        }
    }
    private void SetLifeStockColor(int _playerIndex)
    {
        lifeStockUI[_playerIndex].SetUpColor(_playerIndex); //Only set up color

    }
    public void MinusLifeStock(int _index)
    {
        //update ui and data
        playerLifeStock[_index]--;
        LocalRoomManager.instance.players[_index].SetProperty(CustomPropertyCode.LIFESTOCK, playerLifeStock[_index]);
        SetLifeStockColor(_index);
        PunSendLifeStockChangeEvent(_index, playerLifeStock[_index]);
        lifeStockUI[_index].SetLifeCount(playerLifeStock[_index]);
        lifeStockUI[_index].TriggerReviveAnimation();
    }

    public void CheckPlayerRevive(int _i)
    {
        MinusLifeStock(_i);
        if (PhotonNetwork.IsConnected)
        {
            //Online
            Player _deadPlayer = LocalRoomManager.instance.players[_i].GetValue<Player>(CustomPropertyCode.PLAYER);
            if (_deadPlayer != PhotonNetwork.LocalPlayer) { return; }
            if ((int)_deadPlayer.CustomProperties[CustomPropertyCode.LIFESTOCK] <= 0)
            {
                Debug.Log("player " + _i + " lose !");
                LocalRoomManager.instance.players[_i].SetProperty(CustomPropertyCode.ISLOSE, true);
            }
            else
            {
                //create player:
                PlayerControl _player = LocalRoomManager.instance.InstantiateOnlinePlayer(_i);
                /*
                string playerGCKey = "Player" + _i;
                PlayerControl _player = GCManager.Instantiate(playerGCKey).GetComponent<PlayerControl>();
                _player.GetComponent<PhotonView>().RPC("SetUpOnline", RpcTarget.All, _i);
                _player.transform.position = MapControl.instance.viewWorldCenter;*/
                _player.AddRevive();
            }
        }
        else
        {
            //Local
            if (LocalRoomManager.instance.players[_i].GetValue<int>(CustomPropertyCode.LIFESTOCK) <= 0)
            {
                //player i lose!
                Debug.Log("player " + _i + " lose !");
                LocalRoomManager.instance.players[_i].SetProperty(CustomPropertyCode.ISLOSE, true);
            }
            else
            {
                //create player:
                LocalRoomManager.instance.Revive(_i);
                /*
                string playerGCKey = "Player" + _i;
                PlayerControl _player = GCManager.Instantiate(playerGCKey).GetComponent<PlayerControl>();
                _player.transform.position = MapControl.instance.viewWorldCenter;
                _player.AddRevive();*/
            }
        }
    }

    public void CheckTeamWinLose(GameObject target, GameObject killer)
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            //Online Check only do on master
            StartCoroutine(CheckOnlineTeamWinLoseCoro(target, killer));
        }
        else if (!PhotonNetwork.IsConnected)
        {
            //Local Check
            StartCoroutine(CheckTeamWinLoseCoro(target, killer));
        }
    }
    public IEnumerator CheckTeamWinLoseCoro(GameObject _target, GameObject _killer)
    {
        yield return new WaitForFixedUpdate();
        PlayerControl kpc = _killer.GetComponent<PlayerControl>();

        if ( IsAllPlayerDead()) {
            //every killed by camera
            yield return new WaitForSeconds(3);
            EndGamePlay();
        }

        if (kpc == null)
            yield break;

        int killer_teamCode = (int)LocalRoomManager.instance.players[kpc.dataIndex].playerProperty[CustomPropertyCode.TEAM_CODE];

        int[] other_teams = LocalRoomManager.instance.Get_Index_In_Different_Team(killer_teamCode);

        bool is_team_win = true;
        for (int i = 0; i < other_teams.Length; i++)
        {
            //if (playerLifeStock[other_teams[i]] > 1)
            if (LocalRoomManager.instance.players[other_teams[i]].GetValue<int>(CustomPropertyCode.LIFESTOCK) > 1)
            {
                is_team_win = false;
            }
        }
        if (is_team_win)
        {
            //WIN
            Debug.Log("Winner is team " + killer_teamCode);
            //TODO: win hint change to result scene
            //StartCoroutine(EndGamePlayCoro(3));
            yield return new WaitForSeconds(3);
            EndGamePlay();
        }
    }
    private bool IsAllPlayerDead()
    {
        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            if (LocalRoomManager.instance.players[i].GetValue<int>(CustomPropertyCode.LIFESTOCK) > 1)
            {
                return false;
            }
        }
        return true;

    }

    public IEnumerator CheckOnlineTeamWinLoseCoro(GameObject _target, GameObject _killer)
    {
        yield return new WaitForFixedUpdate();
        PlayerControl kpc = _killer.GetComponent<PlayerControl>();

        if (kpc == null)
        {
            yield break;
        }
        Player _killer_player = LocalRoomManager.instance.players[kpc.dataIndex].GetValue<Player>(CustomPropertyCode.PLAYER);
        int killer_teamCode = (int)_killer_player.CustomProperties[CustomPropertyCode.TEAM_CODE];

        List<Player> _otherTeamsPlayers = LocalRoomManager.instance.Get_Players_In_Different_Team_Online(killer_teamCode);

        bool is_team_win = true;
        for (int i = 0; i < _otherTeamsPlayers.Count; i++)
        {
            if ((int)_otherTeamsPlayers[i].CustomProperties[CustomPropertyCode.LIFESTOCK] > 1)
            {
                is_team_win = false;
            }
        }
        if (is_team_win)
        {
            //WIN
            Debug.Log("Winner is team " + killer_teamCode);
            //TODO: win hint change to result scene
            yield return new WaitForSeconds(3);
            EndGamePlay();
        }
    }
    //Temp
    void EndGamePlay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
    }

}
