using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{

    public SpriteMaskPair spriteMask;
    Material _sprite_mat;
    //TODO: attack action
    Animator animator;

    string _temp_sprite_name;
    public void Start()
    {
        _sprite_mat = new Material(Shader.Find("Unlit/SpriteMask"));
        GetComponent<SpriteRenderer>().material = _sprite_mat;
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

    public void PlayAnimatiom(string name)
    {

        if (spriteMask != null && (_temp_sprite_name != name))
        {
            _temp_sprite_name = name;

        }
        _sprite_mat.SetTexture("_Mask", spriteMask.GetSprite(name).texture);

        animator.Play(name);
    }
}
