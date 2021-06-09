using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    //private static event Action<bool> OnCheckIsAllReady;
    public const string READY = "READY";

    private static GameObject _startBtn;
    private static GameObject _btnHint;
    private PlayerSlot playerSlot;

    [SerializeField]
    private bool _isReady = false;
    private void Start()
    {
        playerSlot = GetComponentInParent<PlayerSlot>();
        //find btn
        if (_startBtn == null)
        {
            _startBtn = GameObject.FindGameObjectWithTag("StartBtn");
            _startBtn.SetActive(false);
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
        LocalReady();

    }

    void ChangeReadyColor()
    {
        //temp
        if (_isReady)
        {
            GetComponent<Image>().color = Color.green;
        }
        else
        {
            GetComponent<Image>().color = Color.white;
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
    }

    private int GetTeamCount()
    {
        int _count = 0;
        Dictionary<int, int> m_teamCount = new Dictionary<int, int>();
        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            if (m_teamCount.ContainsKey((int)LocalRoomManager.instance.players[i].playerProperty[CustomPropertyCode.TEAM_CODE]))
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
