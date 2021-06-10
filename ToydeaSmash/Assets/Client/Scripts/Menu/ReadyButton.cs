using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviourPunCallbacks
{
    //private static event Action<bool> OnCheckIsAllReady;
    public const string READY = "READY";

    private static GameObject _startBtn;
    private static GameObject _btnHint;
    private PlayerSlot playerSlot;

    [SerializeField]
    private bool _isReady = false;
    static Dictionary<Player, bool> m_playerReadyState = new Dictionary<Player, bool>();

    private void Start()
    {
        playerSlot = GetComponentInParent<PlayerSlot>();
        //find btn
        if (_startBtn == null)
        {
            _startBtn = GameObject.FindGameObjectWithTag("StartBtn");
            _startBtn.SetActive(false);
            Debug.Log("start set false");
        }
        if (_btnHint == null)
        {
            _btnHint = GameObject.FindGameObjectWithTag("HintBtn");
            _btnHint.SetActive(false);
        }
        Debug.Log(_btnHint == null);
    }

    public void LocalReady()
    {
        _isReady = !_isReady;
        LocalRoomManager.instance.players[playerSlot.player_index].SetProperty(READY, _isReady);
        //ChangeReadyColor();
        CheckAllLocalPlayerIsReady();
    }
    public void OnlineReady()
    {
        _isReady = !_isReady;
        LocalRoomManager.instance.players[playerSlot.player_index].SetProperty(READY, _isReady);

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        Debug.Log("ready btn update");
        object _isReady;
        if (changedProps.TryGetValue(READY, out _isReady))
        {
            //log to dict
            if (m_playerReadyState.ContainsKey(targetPlayer))
            {
                m_playerReadyState[targetPlayer] = (bool)_isReady;
            }
            else
            {
                m_playerReadyState.Add(targetPlayer, (bool)_isReady);
            }
            Debug.Log("Is maseter? " + PhotonNetwork.IsMasterClient);
            if (!(bool)_isReady)
            {
                _startBtn.SetActive(false);
                return;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                CheckAllOnlinePlayerIsReady();
            }
        }

    }



    public void CheckAllLocalPlayerIsReady()
    {
        bool _isAllReady = true;
        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            Debug.Log("player " + i + " " + LocalRoomManager.instance.players[i].GetValue<bool>(READY));
            if (!LocalRoomManager.instance.players[i].GetValue<bool>(READY))
            {
                _isAllReady = false;
                break;
            }
        }
        //Check team count:        
        ShowAllReadyBtn(_isAllReady);


    }

    public void CheckAllOnlinePlayerIsReady()
    {
        bool _isAllReady = false;
        if (m_playerReadyState.Keys.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            foreach (KeyValuePair<Player, bool> _pair in m_playerReadyState)
            {
                if (!_pair.Value)
                {
                    _isAllReady = false;
                    break;
                }
            }
            _isAllReady = true;
        }

        ShowAllReadyBtn(_isAllReady);
    }

    private void ShowAllReadyBtn(bool _res)
    {
        if (GetTeamCount() > 1 || LocalRoomManager.instance.players.Count == 1)
        {
            _startBtn.SetActive(_res);
        }
        else if (_res)
        {
            //Open error Hint
            _btnHint.SetActive(true);
        }
        else
        {
            _startBtn.SetActive(false);
        }
    }

    private int GetTeamCount()
    {
        int _count = 0;
        Dictionary<int, int> m_teamCount = new Dictionary<int, int>();
        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            if (m_teamCount.ContainsKey(LocalRoomManager.instance.players[i].GetValue<int>(CustomPropertyCode.TEAM_CODE)))
            {
                m_teamCount[(int)LocalRoomManager.instance.players[i].playerProperty[CustomPropertyCode.TEAM_CODE]] += 1;
            }
            else
            {
                m_teamCount.Add((int)LocalRoomManager.instance.players[i].playerProperty[CustomPropertyCode.TEAM_CODE], 1);

            }
        }
        return m_teamCount.Keys.Count;
    }

}
