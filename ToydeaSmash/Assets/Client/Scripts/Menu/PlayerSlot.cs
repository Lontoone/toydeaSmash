using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

using System;
public class PlayerSlot : MonoBehaviourPunCallbacks
{
    public const byte PUN_SLOT_READY_EVENT_CODE = 2;
    //public event Action<LocalPlayerProperty> OnValueChange;
    public event Action<int> OnTeamChanged;
    public event Action<int> OnHeadChanged;
    public event Action<int> OnBodyChanged;

    public int player_index = 0;

    public TextMeshProUGUI name_text;
    public GameObject readyCanvas;
    public GameObject readyHint;
    public GameObject unReadyHint;
    public GameObject btn_group;
    public GameObject readyOkHint;

    public static Head[] heads_res;
    public static Body[] body_res;

    int current_team = 0;
    int current_head = 0;
    int current_body = 0;
    Player thisPlayer;

    public GameObject head;
    public GameObject body;

    //PhotonView pv;

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
    public void SetUpPlayer(Player _p, int _index) //call from player slot manager
    {
        Debug.Log("slot set up Player " + _p.NickName + " local " + PhotonNetwork.LocalPlayer.NickName);

        thisPlayer = _p;
        player_index = _index;
        InitPropertyDict();
        name_text.text = _p.NickName;
        if (_p == PhotonNetwork.LocalPlayer)
        {
            btn_group.SetActive(true);
        }
        else
        {
            btn_group.SetActive(false);
            Destroy(readyHint);
            Destroy(unReadyHint);
        }
    }

    void InitPropertyDict()
    {
        //dict init
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.HEAD_CDOE, current_head);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.BODY_CODE, current_body);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.TEAM_CODE, current_team);

        SetTeam(0);
    }



    //Online
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //Don't apply to self, or it will get crunchy!!
        if (targetPlayer == null || targetPlayer != thisPlayer) { return; }
        Debug.Log("Photon OnPlayerPropertiesUpdate " + "target name " + targetPlayer.NickName + "\n this name " + thisPlayer.NickName + " \n" + (targetPlayer != thisPlayer));

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        object _data;
        //Debug.Log("changedProps : TEAM " + changedProps.ContainsKey(CustomPropertyCode.TEAM_CODE) + " value " + (int)changedProps[CustomPropertyCode.TEAM_CODE]);

        if (changedProps.TryGetValue(CustomPropertyCode.TEAM_CODE, out _data))
        {
            //set team
            Debug.Log(thisPlayer.NickName + " set team");
            SetTeam((int)_data);
        }
        else if (changedProps.TryGetValue(CustomPropertyCode.BODY_CODE, out _data))
        {
            //set weapon
            SetWeapon((int)_data);
        }
        else if (changedProps.TryGetValue(CustomPropertyCode.HEAD_CDOE, out _data))
        {
            //set head
            SetHead((int)_data);
        }
        else
        {
            Debug.Log(targetPlayer.NickName + " Not getting desire data " + changedProps.Keys.Count);
        }

    }

    public void SetReady(bool _isReady)
    {
        if (readyHint != null)
        {
            readyHint.SetActive(!_isReady);
        }
        if (unReadyHint != null)
        {
            unReadyHint.SetActive(_isReady);
        }
        btn_group.SetActive(!_isReady);

        readyOkHint.SetActive(_isReady);
        SetColor(LocalRoomManager.instance.players[player_index].GetValue<int>(CustomPropertyCode.TEAM_CODE));
    }
    public void SetColor(int _colorIndex)
    {
        readyOkHint.GetComponent<UnityEngine.UI.Image>().color = CustomPropertyCode.TEAMCOLORS[_colorIndex];
    }

    public void Team_btn(int _opt)
    {
        current_team = Mathf.Clamp(current_team + _opt, 0, CustomPropertyCode.TEAMCOLORS.Length - 1);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.TEAM_CODE, current_team);
        SetTeam(current_team);
    }
    public void Weapon_btn(int _opt)
    {
        //temp
        current_body = Mathf.Clamp(current_body + _opt, 0, 2);
        //TODO:[BUG] should store int but it is storing name of Resources path.
        //LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.BODY_CODE, body_res[current_body].name);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.BODY_CODE, current_body);

        OnBodyChanged?.Invoke(current_body);
        SetWeapon((int)current_body);
        /*
        *  FOR FURTURE
       current_body = Mathf.Clamp(current_body + _opt, 0, body_res.Length - 1);
       SetWeapon(current_body);
       */
    }
    public void Head_btn(int _opt)
    {
        //temp
        current_head = Mathf.Clamp(current_head + _opt, 0, 8);
        OnHeadChanged?.Invoke(current_head);
        //TODO:[BUG] should store int but it is storing name of Resources path.
        //LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.HEAD_CDOE, heads_res[current_head].name);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.HEAD_CDOE, current_head);
        SetHead((int)current_head);

        /*
        *  FOR FURTURE
        current_head = Mathf.Clamp(current_head + _opt, 0, heads_res.Length - 1);
        SetHead(current_head);
        */
    }

    public void UpdateTeam()
    {
        //SetOtherProperty(CustomPropertyCode.TEAM_CODE, current_team);
    }

    public void SetOtherProperty(string _key, object _data)
    {
        //LocalRoomManager.instance.players[player_index].SetProperty(READY, _isReady);
        if (thisPlayer != null)
        {
            SendCP(_key, _data);
        }
    }

    void SetTeam(int _index)
    {
        //current_team = Mathf.Clamp(_index, 0, CustomPropertyCode.TEAM_CODE.Length - 1);
        current_team = _index;
        head.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[current_team];
        body.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[current_team];
        OnTeamChanged?.Invoke(current_team);
    }

    void SetHead(int _index)
    {
        current_head = _index;
        GameObject _new_head = Instantiate(heads_res[current_head], head.transform.position, Quaternion.identity, head.transform.parent).gameObject;
        Destroy(head);
        head = _new_head;
        head.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[LocalRoomManager.instance.players[player_index].GetValue<int>(CustomPropertyCode.TEAM_CODE)];
        OnHeadChanged?.Invoke(current_head);
    }
    void SetWeapon(int _index)
    {
        current_body = _index;

        GameObject _new_body = Instantiate(body_res[current_body], body.transform.position, Quaternion.identity, head.transform.parent).gameObject;
        Destroy(body);
        body = _new_body;
        Destroy(body.GetComponent<PlayerAttackControl>());
        body.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[LocalRoomManager.instance.players[player_index].GetValue<int>(CustomPropertyCode.TEAM_CODE)];
        OnBodyChanged?.Invoke(current_body);
    }

    void SendCP(string _key, object _data)
    {
        thisPlayer.SetCustomProperties(MyExtension.WrapToHash(new object[]
          { _key, _data }
      ));
    }


}
