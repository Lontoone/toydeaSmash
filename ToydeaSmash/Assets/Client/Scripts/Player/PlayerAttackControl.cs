using Photon.Pun;
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
    public PlayerControl _player;
    private Rigidbody2D rigid;
    private Body body;
    private PhotonView _pv;

    public void Start()
    {
        //include other player's layer:
        for (int i = 0; i < 4; i++)
        {
            string _layer_name = "Player" + i.ToString();
            if (transform.parent != null &&
                LayerMask.NameToLayer(_layer_name) != transform.parent.gameObject.layer)
            {
                targetLayer |= (1 << LayerMask.NameToLayer(_layer_name));
            }
        }

        body = GetComponent<Body>();
        //_player = transform.parent.GetComponent<PlayerControl>();
        //_player = GetComponentInParent<PlayerControl>();
        _pv = GetComponent<PhotonView>();
        //temp:
        /*
        if (_player == null)
        {
            Destroy(gameObject);
        }*/

        _filter.SetLayerMask(targetLayer);
        _filter.useTriggers = true;

        //set up function
        SetupAction();
        if (_pv == null || _pv.IsMine) //if not online or if is online and is mine
        {
            actionController = transform.parent.GetComponent<ActionController>();
            rigid = GetComponent<Rigidbody2D>();
        }
        else if (_pv != null && _pv.IsMine)
        {
            SetupOnlineAction();
        }
    }

    public void Update()
    {
        if (_pv != null && !_pv.IsMine || _player == null)
        {
            return;
        }
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

    private void SetupAction()
    {
        attack.action.AddListener(delegate
        {
            NormalAttack();
        });

        up_attack.action.AddListener(delegate
        {
            UpAttack();
        });

        down_attack.action.AddListener(delegate
        {
            DownAttack();
        });
    }
    private void SetupOnlineAction()
    {
        attack.action.AddListener(delegate
        {
            DoRpcOnAllOtherPlayers("NormalAttack");
            //DoRpcOnAllOtherPlayers("NormalAttack");
            //NormalAttack();
        });

        up_attack.action.AddListener(delegate
        {
            DoRpcOnAllOtherPlayers("UpAttack");
            //DoRpcOnAllOtherPlayers("UpAttack");
            //UpAttack();
        });

        down_attack.action.AddListener(delegate
        {
            DoRpcOnAllOtherPlayers("DownAttack");
            //DoRpcOnAllOtherPlayers("DownAttack");
            //DownAttack();
        });
    }
    [PunRPC]
    public virtual void AttackCheck()
    {
        //check collider
        int _num = current_Attack_collider.OverlapCollider(_filter, _res);
        for (int i = 0; i < _num; i++)
        {
            HitableObj.Hit_event_c(_res[i].gameObject, body.damage, body.transform.parent.gameObject);
            Debug.Log("Hits " + _res[i].gameObject.name);
        }
    }
    [PunRPC]
    public virtual void NormalAttack()
    {
        current_Attack_collider = attackCollider;
        _player.Effect("attack effect", "attack effect", transform.rotation);
        SFXManager.instance.PlaySoundInstance(SFXManager.ATTACK);
        AttackCheck();
        _player.PlayAniamtion("Attack");
    }
    [PunRPC]
    public virtual void UpAttack()
    {
        current_Attack_collider = up_attackCollider;
        AttackCheck();
        _player.PlayAniamtion("Up attack");
        CameraControl.CameraShake(0.25f, 1);
        SFXManager.instance.PlaySoundInstance(SFXManager.HEAVY_PUNCH);
        _player.Effect("up attack", "up attack", transform.rotation);
    }
    [PunRPC]
    public void DownAttack()
    {
        current_Attack_collider = down_attackCollider;
        AttackCheck();
        _player.PlayAniamtion("Down attack");
        CameraControl.CameraShake(0.25f, 1);
        SFXManager.instance.PlaySoundInstance(SFXManager.HEAVY_PUNCH);
        _player.Effect("down attack", "down attack", transform.rotation);
    }

    public virtual void Defense()
    {
        _player.PlayAniamtion("Defense");
    }

    [PunRPC]
    private void PlayerEffect(string _gcKey, string _clipName)
    {

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


    [PunRPC]
    private void DoRpcOnAllOtherPlayers(string _functionName)
    {
        Debug.Log("other players " + _player._otherPlayers.Length);
        for (int i = 0; i < _player._otherPlayers.Length; i++)
        {
            _pv.RPC(_functionName, _player._otherPlayers[i]);
        }
    }
}
