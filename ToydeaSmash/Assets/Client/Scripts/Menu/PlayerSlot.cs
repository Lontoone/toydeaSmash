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
    int current_body = 0;
    Player thisPlayer;

    public GameObject head;
    public GameObject body;

    //PhotonView pv;
    public GameObject btn_group;

    public void Awake()
    {
        if (heads_res == null)
            heads_res = Resources.LoadAll<Head>("Prefab/Head");
        if (body_res == null)
            body_res = Resources.LoadAll<Body>("Prefab/Body");

        Debug.Log(heads_res.Length);
    }

    //local
    public void SetUpPlayer(int _index)
    {
        player_index = _index;
        InitPropertyDict();
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

    void InitPropertyDict()
    {

        //dict init
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.HEAD_CDOE, heads_res[current_head].name);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.BODY_CODE, body_res[current_head].name);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.TEAM_CODE, current_team);

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
        else if (changedProps.TryGetValue(CustomPropertyCode.BODY_CODE, out _data))
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
        current_body = Mathf.Clamp(current_body + _opt, 0, body_res.Length - 1);
        //SendCP(CustomPropertyCode.WEAPON_CODE, current_weapon); //TODO: use event to send data
        SetWeapon(current_body);
    }
    public void Head_btn(int _opt)
    {
        current_head = Mathf.Clamp(current_head + _opt, 0, heads_res.Length - 1);
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

        current_head = _index;
        GameObject _new_head = Instantiate(heads_res[current_head], head.transform.position, Quaternion.identity, head.transform.parent).gameObject;
        Destroy(head);
        head = _new_head;


        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.HEAD_CDOE, heads_res[current_head].name);

    }
    void SetWeapon(int _index)
    {

        current_body = _index;
        GameObject _new_body = Instantiate(body_res[current_body], body.transform.position, Quaternion.identity, head.transform.parent).gameObject;
        Destroy(body);
        body = _new_body;

        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.BODY_CODE, body_res[current_body].name);

    }

    void SendCP(string _key, object _data)
    {
        thisPlayer.SetCustomProperties(MyExtension.WrapToHash(new object[]
          { _key, _data }
      ));
    }


}
