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

    //public event Action<LocalPlayerProperty> OnValueChange;
    public event Action<int> OnTeamChanged;
    public event Action<int> OnHeadChanged;
    public event Action<int> OnBodyChanged;

    public int player_index = 0;

    public TextMeshProUGUI name_text;

    public static Head[] heads_res;
    public static Body[] body_res;

    public Color[] teamColors = new Color[] { Color.red, Color.white, Color.green };

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
        InitMererial();
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

        SetTeam(0);
    }
    void InitMererial()
    {
        /*
        //create head and body metarial
        Material head_mat = new Material(Shader.Find("Unlit/SpriteMask"));
        Material body_mat = new Material(Shader.Find("Unlit/SpriteMask"));
        //load mask sprite from resources
        head.GetComponent<SpriteRenderer>().material = head_mat;
        body.GetComponent<SpriteRenderer>().material = body_mat;

        head_mat.SetTexture("_Mask", Head.LoadMask(heads_res[current_head].name).texture);
        body_mat.SetTexture("_Mask", Body.LoadMask(body_res[current_body].name).texture);

        Debug.Log("Mask " + body_res[current_body].name);*/

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
        SetTeam(current_team);
    }
    public void Weapon_btn(int _opt)
    {
        //temp
        current_body = Mathf.Clamp(current_body + _opt, 0, 2);
        OnBodyChanged?.Invoke(current_body);
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
        /*
        *  FOR FURTURE
        current_head = Mathf.Clamp(current_head + _opt, 0, heads_res.Length - 1);
        SetHead(current_head);
        */
    }

    void SetTeam(int _index)
    {
        current_team = Mathf.Clamp(_index, 0, CustomPropertyCode.TEAM_CODE.Length - 1);

        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.TEAM_CODE, current_team);

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
        head.GetComponent<SpriteRenderer>().color = LocalRoomManager.instance.players[player_index].GetValue<Color>(CustomPropertyCode.TEAM_CODE);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.HEAD_CDOE, heads_res[current_head].name);
        OnHeadChanged?.Invoke(current_head);
    }
    void SetWeapon(int _index)
    {
        current_body = _index;
        GameObject _new_body = Instantiate(body_res[current_body], body.transform.position, Quaternion.identity, head.transform.parent).gameObject;
        Destroy(body);
        body = _new_body;
        Destroy(body.GetComponent<PlayerAttackControl>());
        body.GetComponent<SpriteRenderer>().color = LocalRoomManager.instance.players[player_index].GetValue<Color>(CustomPropertyCode.TEAM_CODE);
        OnBodyChanged?.Invoke(current_body);
        LocalRoomManager.instance.players[player_index].SetProperty(CustomPropertyCode.BODY_CODE, body_res[current_body].name);
    }

    void SendCP(string _key, object _data)
    {
        thisPlayer.SetCustomProperties(MyExtension.WrapToHash(new object[]
          { _key, _data }
      ));
    }


}
