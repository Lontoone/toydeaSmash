using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System;
public class PlayerSlot : MonoBehaviourPunCallbacks
{

    public int player_index = 0;
    public event Action<LocalPlayerProperty> OnValueChange;

    public TextMeshProUGUI name_text;

    public static Head[] heads_res;
    public static Body[] body_res;

    int current_team = 0;
    int current_head = 0;
    int current_weapon = 0;
    Player thisPlayer;

    public GameObject head;
    public GameObject body;

    //PhotonView pv;
    public GameObject btn_group;

    public GameObject[] heads;
    public GameObject[] weapons;

    public void Start()
    {
        //pv = GetComponent<PhotonView>();
        //hide btns if is not mine.
        //if (!pv.IsMine)

        if (heads_res.Length < 1)
            heads_res = Resources.LoadAll<Head>("Prefab/Head");
        if (body_res.Length < 1)
            body_res = Resources.LoadAll<Body>("Prefab/Body");

        Debug.Log(heads_res.Length);
    }

    //local
    public void SetUpPlayer(int _index)
    {
        player_index = _index;
    }
    //online
    public void SetUpPlayer(Player _p) //call from player slot manager
    {

        thisPlayer = _p;
        name_text.text = _p.NickName;

        if (thisPlayer == PhotonNetwork.LocalPlayer)
        {
            btn_group.SetActive(false);
        }

    }

    //Online
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

        //Don't apply to self, or it will get crunchy!!
        if (targetPlayer != thisPlayer) { return; }

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        object _data;
        if (changedProps.TryGetValue(CustomPropertyCode.TEAM_CODE, out _data))
        {
            //set team
            SetTeam((int)_data);
        }
        else if (changedProps.TryGetValue(CustomPropertyCode.WEAPON_CODE, out _data))
        {
            //set weapon
            SetWeapon((int)_data);
        }
        else if (changedProps.TryGetValue(CustomPropertyCode.HEAD_CDOE, out _data))
        {
            //set weapon
            SetHead((int)_data);
        }


    }

    public void Team_btn(int _opt)
    {
        current_team = Mathf.Clamp(current_team + _opt, 0, 3);
        //SendCP(CustomPropertyCode.TEAM_CODE, current_team); //TODO: use event to send data
        SetTeam(current_team);
    }
    public void Weapon_btn(int _opt)
    {
        current_weapon = Mathf.Clamp(current_weapon + _opt, 0, weapons.Length - 1);
        //SendCP(CustomPropertyCode.WEAPON_CODE, current_weapon); //TODO: use event to send data
        SetWeapon(current_weapon);
    }
    public void Head_btn(int _opt)
    {
        current_head = Mathf.Clamp(current_head + _opt, 0, heads.Length - 1);
        //SendCP(CustomPropertyCode.HEAD_CDOE, current_head);  //TODO: use event to send data
        SetHead(current_head);
    }

    void SetTeam(int _index)
    {
        current_team = _index;

        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.TEAM_CODE, current_team);
        //TODO: Set UI

    }

    void SetHead(int _index)
    {
        heads[current_head].SetActive(false);
        current_head = _index;
        heads[current_head].SetActive(true);

        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.HEAD_CDOE, current_head);

    }
    void SetWeapon(int _index)
    {
        weapons[current_weapon].SetActive(false);
        current_weapon = _index;
        weapons[current_weapon].SetActive(true);

        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.WEAPON_CODE, current_weapon);

    }

    void SendCP(string _key, object _data)
    {
        thisPlayer.SetCustomProperties(MyExtension.WrapToHash(new object[]
          { _key, _data }
      ));
    }


}
