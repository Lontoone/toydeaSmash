﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{

    public SpriteMaskPair spriteMask;
    Material _sprite_mat;
    //TODO: attack action
    Animator animator;
    [HideInInspector]
    public SpriteRenderer sp;

    public float damage = 100;
    string _temp_sprite_name;

    public void Awake()
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

        animator = GetComponent<Animator>();

    }


    public static Body LoadBody(string _body_name)
    {
        return Resources.Load<Body>("Prefab/Body/" + _body_name);
    }

    public static Sprite LoadMask(string _body_name)
    {
        Sprite _sp = Resources.Load<Sprite>("img/Mask/Body/" + _body_name);
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


    public void CreateImageTrail()
    {
        AfterImage.CreateImageTrail(sp.sprite, transform.position, transform.lossyScale,sp.color);
        //AfterImage.CreateImageTrail(body.sp.sprite, body.transform.position, body.transform.lossyScale);
    }
}
