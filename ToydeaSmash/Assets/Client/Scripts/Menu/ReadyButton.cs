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
    private PlayerSlot playerSlot;
    private bool _isReady = false;
    public void Start()
    {
        playerSlot = GetComponentInParent<PlayerSlot>();
        if (_startBtn == null)
        {
            _startBtn = GameObject.FindGameObjectWithTag("StartBtn");
            _startBtn.SetActive(false);
        }
    }

    public void LocalReady()
    {
        _isReady = !_isReady;
        LocalRoomManager.instance.players[playerSlot.player_index].SetProperty(READY, _isReady);
        ChangeReadyColor();
        CheckAllLocalPlayerIsReady();
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
            if (!LocalRoomManager.instance.players[i].GetValue<bool>(READY))
            {
                _isAllReady = false;
            }
        }
        //OnCheckIsAllReady?.Invoke(_isAllReady);
        ShowAllReadyBtn(_isAllReady);
    }

    void ShowAllReadyBtn(bool _res)
    {
        _startBtn.SetActive(_res);

    }
}
