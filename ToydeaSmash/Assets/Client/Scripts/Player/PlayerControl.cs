﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//玩家控制
public class PlayerControl : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 10;
    Animator animator;
    Rigidbody2D rigid;
    PhysicsControlListeners listeners;
    HitableObj hitable;
    ActionController actionController;
    public ActionController.mAction idle, walk, hurt, jump_start, jumping, falling, jump_end, doubleJump;

    public string horizontal_axis_name = "Horizontal";  //default
    public string vertical_axis_name = "Vertical";      //default
    //public string jump_axis_name = "Jump";                //default
    public KeyCode jump_key;

    public Head head;
    public Body body;
    SpriteRenderer head_sp, body_sp;

    [SerializeField]
    int jump_count = 0; //跳躍次數 (for 2段跳)
    private void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        listeners = gameObject.GetComponent<PhysicsControlListeners>();
        animator = gameObject.GetComponent<Animator>();
        actionController = gameObject.GetComponent<ActionController>();


        hitable = gameObject.GetComponent<HitableObj>();
        actionController.eActionQueueCleared += AddDefault;
        if (hitable != null)
        {
            hitable.Die_event += Die;
            hitable.gotHit_event += Hurt;
        }
        if (listeners != null)
            listeners.eOnTouchGround += ResetJumpCount;
    }
    private void OnDestroy()
    {
        actionController.eActionQueueCleared -= AddDefault;
        if (hitable != null)
        {
            hitable.Die_event -= Die;
            hitable.gotHit_event -= Hurt;
        }
        if (listeners != null)
            listeners.eOnTouchGround -= ResetJumpCount;
    }

    public void SetUp(LocalPlayerProperty _data, int _i)
    {
        Head _newHead =
            Instantiate(
                Head.LoadHead(_data.playerProperty[CustomPropertyCode.HEAD_CDOE] as string).gameObject,
                head.transform.position,
                Quaternion.identity,
                head.transform.parent
                ).GetComponent<Head>();
        Body _newBody =
            Instantiate(
                Body.LoadBody(_data.playerProperty[CustomPropertyCode.BODY_CODE] as string).gameObject,
                body.transform.position,
                Quaternion.identity,
                body.transform.parent
                ).GetComponent<Body>();

        Destroy(head.gameObject);
        Destroy(body.gameObject);

        head = _newHead;
        body = _newBody;

        head_sp = head.GetComponent<SpriteRenderer>();
        body_sp = body.GetComponent<SpriteRenderer>();


        horizontal_axis_name = "h" + _i.ToString();
        vertical_axis_name = "v" + _i.ToString();
        //jump_axis_name = "j" + _i.ToString();
        jump_key = CustomPropertyCode.JumpKeys[_i];

        //set team Layer
        gameObject.layer = LayerMask.NameToLayer("Player" + _data.playerProperty[CustomPropertyCode.TEAM_CODE]);

        int _team_code = (int)_data.playerProperty[CustomPropertyCode.TEAM_CODE];
        Debug.Log("set Team color " + CustomPropertyCode.TEAMCOLORS[_team_code] + " " + _team_code);

        //set team color        
        head.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[_team_code];
        body.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[_team_code];
    }

    private void Update()
    {
        //跳躍
        //if (Input.GetKeyDown(KeyCode.Space) && (jump_count < 2))
        //if (Input.GetAxisRaw(jump_axis_name) != 0 && (jump_count < 2))
        if (Input.GetKeyDown(jump_key) && (jump_count < 2))
        {

            if (jump_count == 0)
            {

                Debug.Log("Jump start");
                jump_count++;
                actionController.AddAction(jump_start);
            }
            //else if (!listeners.isGrounded)
            else
            {
                Debug.Log("Jump double");
                jump_count++;
                actionController.AddAction(doubleJump);
            }
            Debug.Log("jump count" + jump_count);
            //jump_count++;
        }
    }
    public void AddJumpForce()
    {

        rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
        Debug.Log("jump force " + rigid.velocity);
    }
    void AddDefault()
    {
        //actionController.AddAction(idle);
    }
    private void FixedUpdate()
    {
        //動畫判定:
        if (!listeners.isGrounded) //跳躍
        {
            if (rigid.velocity.y < -0.2f)
            {
                Debug.Log("jump: falling");
                actionController.AddAction(falling);
            }
            else //(rigid.velocity.y > 0.1f)
            {
                actionController.AddAction(jumping);
            }
            /*
            else if (rigid.velocity.normalized.y < 0.01f)
            {
                //actionController.AddAction(jumpEpic);
                Debug.Log("epic");
            }*/
            return;
        }
        //else if (Mathf.Abs(rigid.velocity.x) > 1f) //移動
        //else if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.15f) //移動
        else if (Mathf.Abs(Input.GetAxis(horizontal_axis_name)) > 0.15f) //移動
        {
            Debug.Log("add walk " + listeners.isGrounded);
            actionController.AddAction(walk);

            //animator.Play("Walk");
            jump_count = 0; //TODO:暫時的

        }
        else
        {
            actionController.AddAction(idle);
            /*
            if (Mathf.Abs(Input.GetAxis("Horizontal")) == 0)
                actionController.AddAction(idle);
            else
            {
                actionController.AddAction(walk_stop);
            }*/
        }

    }


    public void Move()
    {
        //rigid.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rigid.velocity.y);
        rigid.velocity = new Vector2(Input.GetAxis(horizontal_axis_name) * speed, rigid.velocity.y);
        //左右翻轉:
        if (rigid.velocity.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            //sp.flipX = false;
        }
        else if (rigid.velocity.x < 0)
        {
            //sp.flipX = true;
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        //.sprite = head.spriteMask.GetSprite("Walk");
        //body_sp.sprite = body.spriteMask.GetSprite("Walk");

        head.PlayAnimatiom("Walk");
        body.PlayAnimatiom("Walk");

    }

    public void Idle()
    {

        head.PlayAnimatiom("Idle");
        body.PlayAnimatiom("Idle");
    }

    void Hurt()
    {
        if (hitable.isHitable)
        {
            //被擊退
            actionController.AddAction(hurt);
            //playerAttack.input_s="Hurt";
            //animator.Play("Hurt");
        }
    }
    void Die()
    {
        Debug.Log("玩家死亡");
    }

    void ResetJumpCount()
    {
        Debug.Log("jump: TouchGround");
        jump_count = 0;
        actionController.AddAction(jump_end);
        //animator.Play("Idle");
    }
}
