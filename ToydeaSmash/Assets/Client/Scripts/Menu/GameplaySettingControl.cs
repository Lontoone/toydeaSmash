using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameplaySettingControl : MonoBehaviour
{
    public const string MAP_OPT = "MAP_OPT";
    public const string MINUTES_OPT = "MINUTES_OPT";
    public const string LIFESTOCK_OPT = "LIFESTOCK_OPT";

    private static readonly string[] map_opts = new string[] { "GamePlay" };
    private static readonly int[] minutes_opts = new int[] { 3, 5, 10 };
    private static readonly int[] lifeStock_opts = new int[] { 1, 2, 3, 4, 5, 10, 99 };

    public TextMeshProUGUI mapNameText, minutesText, lifestockText;

    public int map_index = 0;
    public int minutes_index = 0;
    public int life_stock_index = 0;

    public void Start()
    {
        SetMapIndex(0);
        SetMinutesIndex(0);
        SetLifeStockIndex(0);
    }
    public void SetMapIndex(int _optration)
    {
        map_index = Mathf.Clamp(map_index + _optration, 0, map_opts.Length - 1);
        mapNameText.text = map_opts[map_index].ToString();
    }
    public void SetMinutesIndex(int _optration)
    {
        minutes_index = Mathf.Clamp(minutes_index + _optration, 0, minutes_opts.Length - 1);
        minutesText.text = minutes_opts[minutes_index].ToString();
    }
    public void SetLifeStockIndex(int _optration)
    {
        life_stock_index = Mathf.Clamp(life_stock_index + _optration, 0, lifeStock_opts.Length - 1);
        lifestockText.text = lifeStock_opts[life_stock_index].ToString();
    }

    //OK , save the data.
    public void Confirm()
    {
        //set data
        LocalRoomManager.instance.gamePlaySetting.SetProperty(MAP_OPT, map_opts[map_index]);
        LocalRoomManager.instance.gamePlaySetting.SetProperty(MINUTES_OPT, minutes_opts[minutes_index]);
        LocalRoomManager.instance.gamePlaySetting.SetProperty(LIFESTOCK_OPT, lifeStock_opts[life_stock_index]);

        //TODO TEST:直接開始
        LocalRoomManager.instance.StartGamePlay();
    }

    public void Confirm(int _maxIndex, int _minutes_index, int _lifeStock)
    {
        LocalRoomManager.instance.gamePlaySetting.SetProperty(MAP_OPT, map_opts[_maxIndex]);
        LocalRoomManager.instance.gamePlaySetting.SetProperty(MINUTES_OPT, minutes_opts[_minutes_index]);
        LocalRoomManager.instance.gamePlaySetting.SetProperty(LIFESTOCK_OPT, lifeStock_opts[_lifeStock]);

    }
    public void ConfirmOnline()
    {
        PhotonView _pv = GetComponent<PhotonView>();
        //Store to master clientData
        if (PhotonNetwork.IsMasterClient && _pv != null)
        {
            _pv.RPC("Confirm", RpcTarget.All, map_index, minutes_index, life_stock_index);
        }
        //TODO TEST:直接開始
        LocalRoomManager.instance.StartGamePlay();
    }
}
