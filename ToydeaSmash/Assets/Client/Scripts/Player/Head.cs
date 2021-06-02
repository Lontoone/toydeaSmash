using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Head : MonoBehaviour
{
    //TODO: special skill

    public SpriteMaskPair spriteMask;
    Material _sprite_mat;
    Animator animator;
    public UnityEvent buffs;

    PlayerControl _player;

    string _temp_sprite_name;

    public void Awake()
    {
        _player = GetComponent<PlayerControl>();
        
        _sprite_mat = new Material(Shader.Find("Unlit/SpriteMask"));
        GetComponent<SpriteRenderer>().material = _sprite_mat;
        _sprite_mat.renderQueue = 3000;

        if (spriteMask != null)
            _sprite_mat.SetTexture("_Mask", spriteMask.GetSprite("Idle").texture);  //default
        else
        {
            _sprite_mat.SetTexture("_Mask", Resources.Load<Sprite>("img/black").texture);
        }
        

        animator = GetComponent<Animator>();
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

    /*
     Default Buffs
     */

    public void AddDamage(float _persent)
    {
        float _damage = _player.body.damage * _persent;
        _player.body.damage = _damage;
    }
}
