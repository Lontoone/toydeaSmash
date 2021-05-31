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

    ContactFilter2D filter;
    Collider2D[] res=new Collider2D[5];

    public LayerMask targetLayer;

    public void Start()
    {
        actionController = GetComponent<ActionController>();
        rigid = GetComponent<Rigidbody2D>();

        filter.SetLayerMask(targetLayer);

        attack.action.AddListener(delegate { Attack(); });
        up_attack.action.AddListener(delegate { Attack(); });
        down_attack.action.AddListener(delegate { Attack(); });
    }
    public virtual void Attack()
    {

    }
    public virtual void UpAttack()
    {
    }
    public virtual void DownAttack()
    {

    }

}
