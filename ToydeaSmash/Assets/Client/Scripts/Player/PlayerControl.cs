using System.Collections;
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
    [HideInInspector]
    public HitableObj hitable;
    ActionController actionController;
    public ActionController.mAction idle, walk, hurt, jump_start, jumping, falling, jump_end, doubleJump, dash, duck, stop, hurt_falling, revive;
    public ActionController.mAction landing, land_end;

    public string horizontal_axis_name = "Horizontal";  //default
    public string vertical_axis_name = "Vertical";      //default
    //public string jump_axis_name = "Jump";                //default
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode dash_key = KeyCode.LeftShift;
    public KeyCode duck_key = KeyCode.DownArrow;
    public KeyCode attack_key = KeyCode.Z;
    public KeyCode defense_key = KeyCode.X;

    public Head head;
    public Body body;
    SpriteRenderer head_sp, body_sp;

    public float dash_force = 15;
    public float heal_amount = 50;

    [SerializeField]
    int jump_count = 0; //跳躍次數 (for 2段跳)

    int data_index = 0;
    private void Start()
    {


        hitable = gameObject.GetComponent<HitableObj>();
        actionController.eActionQueueCleared += AddDefault;
        if (hitable != null)
        {
            hitable.Die_event += Die;
            //hitable.gotHit_event += Hurt;
            hitable.gotHit_event += OnHurt;
        }
        if (listeners != null)
            listeners.eOnTouchGround += ResetJumpCount;

        listeners.eOnTouchGround += OnJumpEnd;
    }
    private void OnDestroy()
    {
        actionController.eActionQueueCleared -= AddDefault;
        if (hitable != null)
        {
            hitable.Die_event -= Die;
            //hitable.gotHit_event -= Hurt;
            hitable.gotHit_event -= OnHurt;
        }
        if (listeners != null)
            listeners.eOnTouchGround -= ResetJumpCount;

        listeners.eOnTouchGround -= OnJumpEnd;
    }

    public void SetUp(LocalPlayerProperty _data, int _i)
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        listeners = gameObject.GetComponent<PhysicsControlListeners>();
        animator = gameObject.GetComponent<Animator>();
        actionController = gameObject.GetComponent<ActionController>();

        data_index = _i;

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

        head.ApplyBuff();


        //********Set Keys *****************
        horizontal_axis_name = "h" + _i.ToString();
        vertical_axis_name = "v" + _i.ToString();
        //jump_axis_name = "j" + _i.ToString();
        jump_key = CustomPropertyCode.JumpKeys[_i];
        dash_key = CustomPropertyCode.DashKyes[_i];
        duck_key = CustomPropertyCode.DuckKyes[_i];
        attack_key = CustomPropertyCode.AttackKyes[_i];
        defense_key = CustomPropertyCode.DefenseKyes[_i];


        //set team Layer
        gameObject.layer = LayerMask.NameToLayer("Player" + _data.playerProperty[CustomPropertyCode.TEAM_CODE]);

        int _team_code = (int)_data.playerProperty[CustomPropertyCode.TEAM_CODE];
        Debug.Log("set Team color " + CustomPropertyCode.TEAMCOLORS[_team_code] + " " + _team_code);

        //set team color        
        head.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[_team_code];
        body.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[_team_code];

        //Landing animation  (called by manager)
        //AddLanding();
        //Test: 
        //AddRevive();
    }

    private void Update()
    {
        //跳躍
        //if (Input.GetKeyDown(KeyCode.Space) && (jump_count < 2))
        //if (Input.GetAxisRaw(jump_axis_name) != 0 && (jump_count < 2))
        if (Input.GetKeyDown(jump_key) && (jump_count < 2))
        {
            Debug.Log("jump");
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

        //Dash
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        if (Input.GetKeyDown(dash_key))
        {
            actionController.AddAction(dash);
        }

        //Duck
        if (Input.GetKeyDown(duck_key))
        {
            actionController.AddAction(duck);
            cHeal = StartCoroutine(Heal());
        }
        //Duck Finish
        else if (Input.GetKeyUp(duck_key))
        {
            actionController.AddAction(stop);
            StopCoroutine(cHeal);
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

    void OnJumpEnd()
    {
        actionController.AddAction(jump_end);
    }


    public void Move()
    {
        //rigid.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rigid.velocity.y);
        rigid.velocity = new Vector2(Input.GetAxis(horizontal_axis_name) * speed, rigid.velocity.y);

        //左右翻轉:
        if (rigid.velocity.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

        }
        else if (rigid.velocity.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

    }
    public void OnHurt()
    {
        if (hitable.isHitable && !_isHurting)
        {
            //被擊退
            actionController.AddAction(hurt);
            _isHurting = true;
            Debug.Log("HURT!");
        }
        else if (_isHurting)
        {
            //got hit when playing hurt animation
            actionController.AddAction(hurt_falling);

        }
    }
    public void ResetHitCombo()
    {
        _isHurting = false;
        hitable.hit_combo = 0;
    }
    public void Walk_animation()
    {
        head.PlayAnimation("Walk");
        body.PlayAnimation("Walk");
    }

    public void Idle()
    {

        head.PlayAnimation("Idle");
        body.PlayAnimation("Idle");
    }

    public void Jump_start()
    {

        head.PlayAnimation("Jump-Start");
        body.PlayAnimation("Jump-Start");

        Effect("Jump Smoke", "jump smoke");
    }
    public void Jumping()
    {

        head.PlayAnimation("Jumping");
        body.PlayAnimation("Jumping");
    }
    public void Falling()
    {
        head.PlayAnimation("Jumping Falling");
        body.PlayAnimation("Jumping Falling");
    }
    public void Jump_End()
    {
        head.PlayAnimation("Jump-End");
        body.PlayAnimation("Jump-End");
    }

    public void Dash()
    {

        //rigid.velocity = new Vector2(dash_force, rigid.velocity.y);
        rigid.AddForce(dash_force * -transform.right);
        head.PlayAnimation("Dash");
        body.PlayAnimation("Dash");
    }

    public void Duck()
    {
        head.PlayAnimation("Duck");
        body.PlayAnimation("Duck");
    }

    //first time to join the game
    public void AddLanding()
    {
        if (actionController == null)
            actionController = GetComponent<ActionController>();
        actionController.AddAction(landing);
        listeners.eOnTouchGround += AddLandEnd;
        cCreateImageTrail = StartCoroutine(CreateTrailCoro());
    }
    public void AddLandEnd()
    {
        actionController.AddAction(land_end);
        listeners.eOnTouchGround -= AddLandEnd;

        StopCoroutine(cCreateImageTrail);
        //PlayAniamtion("landing fall");
    }
    public void CreateImageTrail()
    {
        head.CreateImageTrail();
        body.CreateImageTrail();
    }

    public void AddRevive()
    {
        actionController.AddAction(revive);
    }
    public void ReviveMove()
    {
        rigid.Sleep();
        Vector3 _move = new Vector3(Input.GetAxisRaw(horizontal_axis_name),
                                                                Input.GetAxisRaw(vertical_axis_name));
        transform.position = transform.position + _move * Time.deltaTime * speed;

        if (_move.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

        }
        else if (_move.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }


    //Heal player HP
    Coroutine cHeal;
    IEnumerator Heal()
    {
        WaitForSeconds _oneSec = new WaitForSeconds(1);
        int _prewait = 2;
        while (_prewait > 0)
        {
            _prewait--;
            yield return _oneSec;
        }
        while (true)
        {
            Animator _effect = GCManager.Instantiate("Heal Effect", position: listeners.footPositon.transform.position).GetComponent<Animator>();
            _effect.Play("heal");
            hitable.Heal(heal_amount);
            yield return _oneSec;
        }
    }

    Coroutine cCreateImageTrail;
    IEnumerator CreateTrailCoro() {
        WaitForFixedUpdate _wait = new WaitForFixedUpdate();
        while (true) {
            CreateImageTrail();
            yield return _wait;
        }
    }

    public void Stop()
    {
        actionController.AddAction(stop);
        rigid.velocity = Vector2.zero;
    }

    void Effect(string _gc_key, string _clip_name)
    {
        GameObject _effect = GCManager.Instantiate(_gc_key);
        _effect.GetComponent<Animator>().Play(_clip_name);
        _effect.transform.position = listeners.footPositon.transform.position;
    }
    [SerializeField]
    bool _isHurting = false;
    public void Hurt()
    {
        //add force
        //rigid.AddForce(transform.right * dash_force * 0.5f);

        PlayAniamtion("Hurt");

    }
    //hit to sky
    public void Hurt_Fly()
    {
        //add force to sky 
        Vector2 _dir = new Vector2(transform.right.x * 0.5f, 1);
        rigid.AddForce(_dir * dash_force);

        PlayAniamtion("Hurt Falling");
    }
    void Die()
    {
        Debug.Log("玩家死亡");

        //disable control
        //actionController.enabled = false;
        hitable.isHitable = false;
        actionController.StopAllCoroutines();
        Destroy(actionController);
        PlayAniamtion("Die");

        Invoke("RecreatePlayer", 3);

        this.enabled = false;
    }
    //create new player 3sec after die
    void RecreatePlayer()
    {
        LocalRoomManager.instance.Revive(data_index);
        Destroy(gameObject);
    }

    void ResetJumpCount()
    {
        Debug.Log("jump: TouchGround");
        jump_count = 0;
        actionController.AddAction(jump_end);
        //animator.Play("Idle");
    }

    public void PlayAniamtion(string _clipName)
    {
        head.PlayAnimation(_clipName);
        body.PlayAnimation(_clipName);
    }
}
