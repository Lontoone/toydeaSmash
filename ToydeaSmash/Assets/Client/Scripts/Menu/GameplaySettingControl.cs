using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameplaySettingControl : MonoBehaviour
{
    public const string MAP_OPT = "MAP_OPT";
    public const string MINUTES_OPT = "MINUTES_OPT";
    public const string LIFESTOCK_OPT = "LIFESTOCK_OPT";
    public const int PUN_ONCONFIRME_EVENTCODE = 1; //todo : put it together?

    private static readonly string[] map_opts = new string[] { "GamePlay", "GamePlay night" };
    private static readonly int[] minutes_opts = new int[] { 3, 5, 10 };
    private static readonly int[] lifeStock_opts = new int[] { 1, 2, 3, 4, 5, 10, 99 };
    private static List<Sprite> s_maps = new List<Sprite>();
    private Dictionary<int, int> s_m_teamLayerAssign = new Dictionary<int, int>(); //team code , layer code
    private const string mapResPath = "scene selection/";

    public TextMeshProUGUI minutesText, lifestockText;
    public Image mapPreview;

    public int map_index = 0;
    public int minutes_index = 0;
    public int life_stock_index = 0;

    public void Start()
    {
        for (int i = 0; i < map_opts.Length; i++)
        {
            s_maps.Add(Resources.Load<Sprite>(mapResPath + map_opts[i]));
        }

        SetMapIndex(0);
        SetMinutesIndex(0);
        SetLifeStockIndex(0);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    public void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    public void SetMapIndex(int _optration)
    {
        map_index = Mathf.Clamp(map_index + _optration, 0, map_opts.Length - 1);
        mapPreview.sprite = s_maps[map_index];
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

        AssignPlayerLayers();
        //Load GamePlay
        SceneManager.LoadScene("GamePlay");
    }
    public void Confirm(int _maxIndex, int _minutes_index, int _lifeStock)
    {
        LocalRoomManager.instance.gamePlaySetting.SetProperty(MAP_OPT, map_opts[_maxIndex]);
        LocalRoomManager.instance.gamePlaySetting.SetProperty(MINUTES_OPT, minutes_opts[_minutes_index]);
        LocalRoomManager.instance.gamePlaySetting.SetProperty(LIFESTOCK_OPT, lifeStock_opts[_lifeStock]);

    }
    public void ConfirmOnline()  //called by btn
    {
        //Store to master clientData
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.LocalPlayer.RPC("Confirm", RpcTarget.All, map_index, minutes_index, life_stock_index);
            object[] _content = new object[] {
                map_index,
                minutes_index,
                life_stock_index
            };
            AssignPlayerLayers();
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(PUN_ONCONFIRME_EVENTCODE, _content, raiseEventOptions, SendOptions.SendReliable);
        }
        //TODO TEST:直接開始
        //LocalRoomManager.instance.StartGamePlay();
    }
    //For Online
    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == PUN_ONCONFIRME_EVENTCODE)
        {
            object[] data = (object[])photonEvent.CustomData;
            Confirm((int)data[0], (int)data[1], (int)data[2]);

            Debug.Log("on scene Confirm");

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GamePlay");
            }
        }
        //PhotonNetwork.LoadLevel(map_opts[map_index]);
    }

    private void AssignPlayerLayers()
    {
        int _layerCounter = 0;
        for (int i = 0; i < LocalRoomManager.instance.players.Count; i++)
        {
            int _teamCode = LocalRoomManager.instance.players[i].GetValue<int>(CustomPropertyCode.TEAM_CODE, PhotonNetwork.IsConnected);
            //check if the layerCode is used with other team
            int _teamLayer = 0;
            //is my team registered?
            if (s_m_teamLayerAssign.TryGetValue(_teamCode, out _teamLayer))
            {
                //get their layer
                LocalRoomManager.instance.players[i].SetProperty(CustomPropertyCode.TEAM_LAYER, _layerCounter);
            }
            else
            {
                //if not registered. get thier layer
                _layerCounter++;
                LocalRoomManager.instance.players[i].SetProperty(CustomPropertyCode.TEAM_LAYER, _layerCounter);
                s_m_teamLayerAssign.Add(_teamCode, _layerCounter);
            }
        }       
    }
}
