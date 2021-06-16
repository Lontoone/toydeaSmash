using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager instance;
    public const string DEATH = "DEATH";
    public const string KILL = "KILL";
    public const string DAMAGE = "DAMAGE";
    public const string DAMAGETAKE = "DAMAGETAKE";
    public const byte PUN_ON_RESULT_EVENT_CODE = 3;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        HitableObj.OnKilled += AddKillAndDeathCount;
        HitableObj.Hit_event += AddDamageCount;
    }
    public void OnDestroy()
    {
        HitableObj.OnKilled -= AddKillAndDeathCount;
        HitableObj.Hit_event -= AddDamageCount;
    }
    void AddKillAndDeathCount(GameObject target, GameObject killer)
    {
        PlayerControl _targetPlayer = target.GetComponent<PlayerControl>();
        PlayerControl _killerPlayer = killer.GetComponent<PlayerControl>();
        if (PhotonNetwork.IsConnected)
        {
            int _index = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.PLAYER_INDEX];
            if (_targetPlayer.dataIndex == _index)
            {
                SetProperty(_targetPlayer, DEATH, 1);
            }
            if (_killerPlayer.dataIndex == _index)
            {
                SetProperty(_killerPlayer, KILL, 1);
            }
        }
        else
        {
            SetProperty(_targetPlayer, DEATH, 1);
            SetProperty(_killerPlayer, KILL, 1);
        }


    }
    void AddDamageCount(GameObject t, float d, GameObject s)
    {
        PlayerControl _targetPlayer = t.GetComponent<PlayerControl>();
        PlayerControl _sourcePlayer = s.GetComponent<PlayerControl>();
        if (PhotonNetwork.IsConnected)
        {
            int _index = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.PLAYER_INDEX];
            if (_targetPlayer.dataIndex == _index)
            {
                SetProperty(_targetPlayer, DAMAGETAKE, (int)d);
            }
            if (_sourcePlayer.dataIndex == _index)
            {
                SetProperty(_sourcePlayer, DAMAGE, (int)d);
            }
        }
        else
        {
            //Local
            SetProperty(_targetPlayer, DAMAGETAKE, (int)d);
            SetProperty(_sourcePlayer, DAMAGE, (int)d);
        }
    }
    void SetProperty(PlayerControl player, string _key, int data_to_add)
    {
        if (player != null)
        {
            object _outdata;
            if (LocalRoomManager.instance.players[player.dataIndex].playerProperty.TryGetValue(_key, out _outdata))
            {
                LocalRoomManager.instance.players[player.dataIndex].SetProperty(_key,
                    (int)_outdata + data_to_add);
            }
            else
            {
                //init value
                LocalRoomManager.instance.players[player.dataIndex].SetProperty(_key, data_to_add);
            }

        }
    }

    /*
    public void PunRaiseWinLoseEvent()
    {

        object[] content = new object[] { new Vector3(10.0f, 2.0f, 5.0f), 1, 2, 5, 10 }; 
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; 
        PhotonNetwork.RaiseEvent(PUN_ON_RESULT_EVENT_CODE, content, raiseEventOptions, SendOptions.SendReliable);
    }*/
}
