using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Head : MonoBehaviour
{
    //TODO: special skill

    public SpriteMaskPair spriteMask;
    [SerializeField]
    Material _sprite_mat;
    Animator animator;
    [HideInInspector]
    public SpriteRenderer sp;
    public UnityEvent buffs;

    PlayerControl _player;

    string _temp_sprite_name;

    public void Awake()
    {
        _player = GetComponent<PlayerControl>();
        SetUpSpriteMaskMatetial();
        animator = GetComponent<Animator>();
    }
    private void SetUpSpriteMaskMatetial()
    {
        sp = GetComponent<SpriteRenderer>();
        _sprite_mat = new Material(Shader.Find("Unlit/SpriteMask"));
        sp.material = _sprite_mat;
        _sprite_mat.renderQueue = 3000;

        if (spriteMask != null)
            _sprite_mat.SetTexture("_Mask", spriteMask.GetSprite("Idle").texture);  //default
        else
        {
            _sprite_mat.SetTexture("_Mask", Resources.Load<Sprite>("img/black").texture);
        }

    }

    public static Head LoadHead(string _head_name)
    {
        return Resources.Load<Head>("Prefab/Head/" + _head_name);
    }
    public static Sprite LoadMask(string _head_name)
    {
        Sprite _sp = Resources.Load<Sprite>("img/Mask/Head/" + _head_name);
        if (_sp == null)
        {
            _sp = Resources.Load<Sprite>("img/black");
        }
        return _sp;

    }

    public void PlayAnimation(string name)
    {

        if (spriteMask != null && (_temp_sprite_name != name))
        {
            _temp_sprite_name = name;
            Sprite _sp = spriteMask.GetSprite(name);
            if (_sp != null)
                _sprite_mat.SetTexture("_Mask", _sp.texture);
        }
        if (animator == null)
            animator = GetComponent<Animator>();
        animator.Play(name);
    }

    public void ApplyBuff()
    {
        if (buffs != null)
        {
            buffs.Invoke();
        }
    }

    [PunRPC]
    public void RpcSetParent(int _playerIndex)
    {
        Debug.Log("Rpc Set Parent index: " + _playerIndex);     
        _player = PlayerControl.FindPlayerControlByIndex(_playerIndex);
        if (_player != null)
        {
            transform.SetParent(_player.transform);
            _player.head = this;
        }
    }

    /*
     Default Buffs
     */

    public void AddDamage(float _persent)
    {
        float _damage = _player.body.damage * _persent;
        _player.body.damage = _damage;
    }

    public void CreateImageTrail()
    {
        AfterImage.CreateImageTrail(sp.sprite, transform.position, transform.lossyScale, sp.color);
        //AfterImage.CreateImageTrail(body.sp.sprite, body.transform.position, body.transform.lossyScale);
    }
}
