using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackControl : MonoBehaviour
{

    public ActionController.mAction attack, up_attack, down_attack, dash, defense;

    public Collider2D current_Attack_collider;

    public Collider2D attackCollider;
    public Collider2D up_attackCollider;
    public Collider2D down_attackCollider;
    public LayerMask targetLayer;

    private ActionController actionController;
    private static Collider2D[] _res = new Collider2D[10]; 
    private ContactFilter2D _filter;
    private PlayerControl _player;
    private Rigidbody2D rigid;
    private Body body;




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
            if (LayerMask.NameToLayer(_layer_name) != transform.parent.gameObject.layer)
            {
                targetLayer |= (1 << LayerMask.NameToLayer(_layer_name));
            }
        }
        _filter.SetLayerMask(targetLayer);
        _filter.useTriggers = true;

        attack.action.AddListener(delegate
        {
            _player.PlayAniamtion("Attack");
            current_Attack_collider = attackCollider;
            Attack();
            _player.Effect("attack effect", "attack effect");
        });

        up_attack.action.AddListener(delegate
        {
            UpAttack();
            Attack();
            _player.Effect("up attack", "up attack");
        });

        down_attack.action.AddListener(delegate
        {
            DownAttack();
            Attack();
            _player.Effect("down attack", "down attack");
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
        else if (Input.GetAxisRaw(_player.vertical_axis_name) < 0 && Input.GetKeyDown(_player.attack_key))
        {
            actionController.AddAction(down_attack);
        }
        //Attack input
        else if (Input.GetKeyDown(_player.attack_key))
        {
            actionController.AddAction(attack);
        }
        //defense
        else if (Input.GetKeyDown(_player.defense_key))
        {
            _player.hitable.damage_taking_rate *= 0.2f;
            actionController.AddAction(defense);
        }

        //defense end
        else if (Input.GetKeyUp(_player.defense_key))
        {
            _player.hitable.damage_taking_rate = 1;
            actionController.AddAction(_player.stop);
        }

    }


    public virtual void Attack()
    {
        //test
        //_attack_test();

        //check collider
        int _num = current_Attack_collider.OverlapCollider(_filter, _res);
        for (int i = 0; i < _num; i++)
        {
            HitableObj.Hit_event_c(_res[i].gameObject, body.damage, body.transform.parent.gameObject);
            Debug.Log("Hits " + _res[i].gameObject.name);
        }
        //effect:
        //GCManager.Instantiate("attack effect", position: transform.position).GetComponent<Animator>().Play("attack effect");

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
        actionController.AddAction(_player.duck);

    }

    public virtual void Defense()
    {
        _player.PlayAniamtion("Defense");
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
