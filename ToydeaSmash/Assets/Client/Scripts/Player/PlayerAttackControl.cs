using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackControl : MonoBehaviour
{

    ActionController actionController;
    Rigidbody2D rigid;
    public ActionController.mAction attack, up_attack, down_attack, dash;

    public Collider2D current_Attack_collider;

    public Collider2D attackCollider;
    public Collider2D up_attackCollider;
    public Collider2D down_attackCollider;

    PlayerControl _player;
    Body body;

    ContactFilter2D filter;
    Collider2D[] res = new Collider2D[5];

    public LayerMask targetLayer;

    public void Start()
    {
        body = GetComponent<Body>();
        _player = transform.parent.GetComponent<PlayerControl>();
        actionController = transform.parent.GetComponent<ActionController>();
        rigid = GetComponent<Rigidbody2D>();

        //include other player's layer:
        for (int i = 0; i < 4; i++)
        {
            string _layer_name = "Player" + i.ToString();
            Debug.Log("layer " + LayerMask.NameToLayer(_layer_name) + " vs " + transform.parent.gameObject.layer);
            if (LayerMask.NameToLayer(_layer_name) != transform.parent.gameObject.layer)
            {
                targetLayer |= (1 << LayerMask.NameToLayer(_layer_name));
            }
        }


        filter.SetLayerMask(targetLayer);
        filter.useTriggers = true;

        attack.action.AddListener(delegate
        {
            _player.PlayAniamtion("Attack");
            current_Attack_collider = attackCollider;
            Attack();
        });

        up_attack.action.AddListener(delegate
        {
            UpAttack();
            Attack();
        });

        down_attack.action.AddListener(delegate
        {
            DownAttack();
            Attack();
        });

        //for test:
        //attack.callbackEvent.AddListener(delegate { _attack_callback_event(); });
    }

    public void Update()
    {
        //Up Attack
        if (Input.GetAxisRaw(_player.vertical_axis_name) > 0 && Input.GetKeyDown(_player.attack_key))
        {
            actionController.AddAction(up_attack);
        }
        //Down Attack
        else if (Input.GetAxisRaw(_player.vertical_axis_name) <0  && Input.GetKeyDown(_player.attack_key))
        {
            actionController.AddAction(down_attack);
        }
        //Attack input
        else if (Input.GetKeyDown(_player.attack_key))
        {
            actionController.AddAction(attack);
        }


    }


    public virtual void Attack()
    {
        //test
        //_attack_test();

        //check collider

        int _num = current_Attack_collider.OverlapCollider(filter, res);
        for (int i = 0; i < _num; i++)
        {
            HitableObj.Hit_event_c(res[i].gameObject, body.damage, body.transform.parent.gameObject);
            Debug.Log("Hits " + res[i].gameObject.name);
        }

        //effect:
        GCManager.Instantiate("attack effect", position: transform.position).GetComponent<Animator>().Play("attack effect");
    }
    public virtual void UpAttack()
    {
        current_Attack_collider = up_attackCollider;
        _player.PlayAniamtion("Up attack");
    }
    public virtual void DownAttack()
    {
        current_Attack_collider = down_attackCollider;
        _player.PlayAniamtion("Down attack");

        //TODO: back to down animation:
        //actionController.AddAction(_player.duck);
    }


    /*   TEST   */
    Color _origin_color;
    void _attack_test()
    {
        SpriteRenderer _sp = GetComponent<SpriteRenderer>();
        _origin_color = _sp.color;

        _sp.color = Random.ColorHSV();
    }

    void _attack_callback_event()
    {
        GetComponent<SpriteRenderer>().color = _origin_color;
    }

}
