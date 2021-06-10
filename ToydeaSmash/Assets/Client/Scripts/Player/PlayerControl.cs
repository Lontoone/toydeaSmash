using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Photon.Pun;
//玩家控制
public class PlayerControl : MonoBehaviour
{
    public static event Action<int> OnCreate;
    public static event Action<int> OnDestory;
    public float speed = 5;
    public float jumpForce = 10;
    [HideInInspector]
    public HitableObj hitable;

    public ActionController.mAction idle, walk, hurt, jump_start, jumping, falling, jump_end, doubleJump, dash, duck, stop, hurt_falling, revive;
    public ActionController.mAction landing, land_end;

    /*control keys*/
    public string horizontal_axis_name = "Horizontal";  //default
    public string vertical_axis_name = "Vertical";      //default
    //public string jump_axis_name = "Jump";            //default
    public KeyCode jump_key = KeyCode.Space;
    public KeyCode dash_key = KeyCode.LeftShift;
    public KeyCode duck_key = KeyCode.DownArrow;
    public KeyCode attack_key = KeyCode.Z;
    public KeyCode defense_key = KeyCode.X;

    public Head head;
    public Body body;

    public float dashForce = 15;
    public float healAmount = 50;
    [HideInInspector]
    public int dataIndex = 0;

    [SerializeField]
    private int jumpCount = 0; //跳躍次數 (for 2段跳)

    [SerializeField]
    private Ease easeType;

    [SerializeField]
    private LayerMask obsticalLayerMask;

    private Rigidbody2D rigid;
    private ActionController actionController;
    private PhysicsControlListeners listeners;

    private Coroutine c_heal;
    private Coroutine cCreateImageTrail;
    private bool _isOnline = false;
    private PhotonView _pv;
    public void Awake()
    {
        SetUpOnline();
    }

    private void Start()
    {
        hitable = gameObject.GetComponent<HitableObj>();
        if (actionController != null)
        {
            actionController.eActionQueueCleared += AddDefault;
        }
        if (hitable != null)
        {
            hitable.Die_event += Die;
            //hitable.gotHit_event += Hurt;
            hitable.gotHit_event += OnHurt;
        }
        if (listeners != null)
        {
            listeners.eOnTouchGround += ResetJumpCount;
            listeners.eOnTouchGround += OnJumpEnd;
        }
    }
    private void OnDestroy()
    {
        if (actionController != null)
        {
            actionController.eActionQueueCleared -= AddDefault;
        }
        if (hitable != null)
        {
            hitable.Die_event -= Die;
            hitable.gotHit_event -= OnHurt;
        }
        if (listeners != null)
            listeners.eOnTouchGround -= ResetJumpCount;
        if (listeners != null)
        {
            listeners.eOnTouchGround -= OnJumpEnd;
            listeners.eOnTouchGround -= ResetJumpCount;
        }
    }
    public void SetUpOnline()
    {
        _pv = GetComponent<PhotonView>();
        if (_pv == null)
        {
            return;
        }
        _isOnline = true;
        if (!_pv.IsMine)
        {
            Destroy(GetComponent<PhysicsControlListeners>());
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<ActionController>());
            Destroy(body.GetComponent<PlayerAttackControl>());
            //Destroy(this);
        }
        else
        {
            rigid = gameObject.GetComponent<Rigidbody2D>();
            listeners = gameObject.GetComponent<PhysicsControlListeners>();
            actionController = gameObject.GetComponent<ActionController>();

            string _head_path = "Prefab/Online/Head/" + PlayerSlot.heads_res[(int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.HEAD_CDOE]].name;
            string _body_path = "Prefab/Online/Body/" + PlayerSlot.body_res[(int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.BODY_CODE]].name;
            Head _newHead =
                             PhotonNetwork.Instantiate(
                             _head_path,
                             head.transform.position,
                             Quaternion.identity
                             ).GetComponent<Head>();
            Body _newBody =
                            PhotonNetwork.Instantiate(
                            _body_path,
                            body.transform.position,
                            Quaternion.identity
                            ).GetComponent<Body>();

            _newBody.transform.SetParent(body.transform.parent);
            _newHead.transform.SetParent(head.transform.parent);
            Destroy(head.gameObject);
            Destroy(body.gameObject);

            head = _newHead;
            body = _newBody;

            //set team Layer
            gameObject.layer = LayerMask.NameToLayer("Player" + PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.TEAM_CODE]);

            int _team_code = (int)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyCode.TEAM_CODE];
            Debug.Log("set Team color " + CustomPropertyCode.TEAMCOLORS[_team_code] + " " + _team_code);

            //set team color        
            head.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[_team_code];
            body.GetComponent<SpriteRenderer>().color = CustomPropertyCode.TEAMCOLORS[_team_code];

            SetKey(0);
        }
    }
    public void SetUp(LocalPlayerProperty _data, int _i)
    {
        OnCreate?.Invoke(_i);

        rigid = gameObject.GetComponent<Rigidbody2D>();
        listeners = gameObject.GetComponent<PhysicsControlListeners>();
        actionController = gameObject.GetComponent<ActionController>();

        dataIndex = _i;

        Head _newHead =
            Instantiate(
                Head.LoadHead(PlayerSlot.heads_res[(int)_data.playerProperty[CustomPropertyCode.HEAD_CDOE]].name).gameObject,
                head.transform.position,
                Quaternion.identity,
                head.transform.parent
                ).GetComponent<Head>();
        Body _newBody =
            Instantiate(
                //Body.LoadBody(_data.playerProperty[CustomPropertyCode.BODY_CODE] as string).gameObject,
                Body.LoadBody(PlayerSlot.body_res[(int)_data.playerProperty[CustomPropertyCode.BODY_CODE]].name).gameObject,
                body.transform.position,
                Quaternion.identity,
                body.transform.parent
                ).GetComponent<Body>();

        Destroy(head.gameObject);
        Destroy(body.gameObject);

        head = _newHead;
        body = _newBody;

        head.ApplyBuff();

        //********Set Keys *****************
        SetKey(_i);


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
    private void SetKey(int index)
    {
        int _i = index % 2;
        horizontal_axis_name = "h" + _i.ToString();
        vertical_axis_name = "v" + _i.ToString();
        //jump_axis_name = "j" + _i.ToString();
        jump_key = CustomPropertyCode.JumpKeys[_i];
        dash_key = CustomPropertyCode.DashKyes[_i];
        duck_key = CustomPropertyCode.DuckKyes[_i];
        attack_key = CustomPropertyCode.AttackKyes[_i];
        defense_key = CustomPropertyCode.DefenseKyes[_i];
    }

    private void Update()
    {
        if (_pv != null && !_pv.IsMine)
        {
            return;
        }

        //跳躍
        //Duck and jump down
        if (Input.GetKey(duck_key) && Input.GetKeyDown(jump_key))
        {
            //check ground is pass throughable?
            GameObject __ground = listeners.currentStandingGround;
            if (__ground != null && __ground.tag == "ThinWall")
                transform.DOMoveY(transform.position.y - 3.5f, 0.1f);
        }
        else if (Input.GetKeyDown(jump_key) && (jumpCount < 1))
        {
            Debug.Log("jump");
            if (jumpCount == 0)
            {
                Debug.Log("Jump start");
                Jump_start();
                AddJumpForce();
                actionController.AddAction(jump_start);
                jumpCount++;
            }
            //else if (!listeners.isGrounded)
            else
            {
                Debug.Log("Jump double");
                actionController.AddAction(doubleJump);
                jumpCount++;
            }
            Debug.Log("jump count" + jumpCount);
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
            c_heal = StartCoroutine(HealCoro());
        }

        //Duck Finish
        else if (Input.GetKeyUp(duck_key))
        {
            actionController.AddAction(stop);
            StopCoroutine(c_heal);
        }

    }
    public void AddJumpForce()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, jumpForce);
        //rigid.AddForce(transform.up*jumpForce*100);
        Debug.Log("jump force " + rigid.velocity);
    }
    void AddDefault()
    {
        //actionController.AddAction(idle);
    }
    private void FixedUpdate()
    {
        if (_pv != null && !_pv.IsMine)
        {
            return;
        }

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
            jumpCount = 0; //TODO:暫時的

        }
        else
        {
            actionController.AddAction(idle);
        }

    }

    void OnJumpEnd()
    {
        actionController.AddAction(jump_end);
    }


    public void Move()
    {
        rigid.velocity = new Vector2(Input.GetAxis(horizontal_axis_name) * speed, rigid.velocity.y);
        //rigid.velocity = new Vector2(Input.GetAxis(horizontal_axis_name) * speed, rigid.velocity.y - Mathf.Pow(rigid.gravityScale, 0.5f));
        /*
        float _x_input = Input.GetAxis(horizontal_axis_name);
        Vector3 _move = new Vector2((_x_input+rigid.velocity.x )* speed * Time.deltaTime, 0);
        transform.position = _move + transform.position;
        Debug.Log("not wall v " + rigid.velocity+" "+_move);*/

        //左右翻轉:
        //if (_x_input> 0)
        if (rigid.velocity.x > 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

        }
        //else if (_x_input< 0)
        else if (rigid.velocity.x < 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }




    }
    public void OnHurt()
    {
        SFXManager.instance.PlaySoundInstance(SFXManager.HURT);
        if (hitable.isHitable && !_isHurting)
        {
            if (listeners.isGrounded)
                actionController.AddAction(hurt);
            else
                actionController.AddAction(hurt_falling);
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
        head.PlayAnimation("Jump Falling");
        body.PlayAnimation("Jump Falling");
        SFXManager.instance.PlaySoundInstance(SFXManager.JUMP);
        Effect("Jump Smoke", "jump smoke");
    }
    public void Jumping()
    {
        head.PlayAnimation("Jumping");
        body.PlayAnimation("Jumping");
    }
    public void Falling()
    {
        head.PlayAnimation("Jump Falling");
        body.PlayAnimation("Jump Falling");

        Debug.Log("falling v " + rigid.velocity);
    }
    public void Jump_End()
    {
        head.PlayAnimation("Jump-End");
        body.PlayAnimation("Jump-End");
    }
    public void LandingEnd()
    {
        head.PlayAnimation("Landing Fall");
        body.PlayAnimation("Landing Fall");
        Effect("landing effect", "Landing");
    }
    public void Dash()
    {
        head.PlayAnimation("Dash");
        body.PlayAnimation("Dash");

        hitable.isHitable = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, dashForce, obsticalLayerMask);
        Vector2 _endPos;

        if (hit.collider != null)
        {
            _endPos = hit.point;
        }
        else
        {
            _endPos = transform.position - transform.right * dashForce;
        }

        DOTween.Sequence().
            Append(transform.DOMove(_endPos, dash.duration)).SetEase(easeType);

        StartCreateTrail();

    }
    public void DashCallBack()
    {
        //reset
        hitable.isHitable = true;
        StopCreateTrail();
    }

    public void Duck()
    {
        head.PlayAnimation("Duck");
        body.PlayAnimation("Duck");
    }
    public void DuckCallback()
    {
        StopCoroutine(c_heal);
        c_heal = null;
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


    IEnumerator HealCoro()
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
            Animator _effect = GCManager.Instantiate("Heal Effect", _position: listeners.footPositon.transform.position).GetComponent<Animator>();
            _effect.Play("heal");
            hitable.Heal(healAmount);
            yield return _oneSec;
        }
    }


    private void StartCreateTrail()
    {
        cCreateImageTrail = StartCoroutine(CreateTrailCoro());
    }
    private void StopCreateTrail()
    {
        if (cCreateImageTrail != null)
        {
            StopCoroutine(cCreateImageTrail);
            cCreateImageTrail = null;
        }
    }
    IEnumerator CreateTrailCoro()
    {
        WaitForFixedUpdate _wait = new WaitForFixedUpdate();
        while (true)
        {
            CreateImageTrail();
            yield return _wait;
        }
    }

    public void Stop()
    {
        actionController.AddAction(stop);
        rigid.velocity = Vector2.zero;
    }

    public void Effect(string _gc_key, string _clip_name, Quaternion _rotation = default)
    {
        GameObject _effect = GCManager.Instantiate(_gc_key, _rotation: _rotation);
        if (_effect != null)
        {
            _effect.GetComponent<Animator>().Play(_clip_name);
            _effect.transform.position = listeners.footPositon.transform.position;
        }
    }
    [SerializeField]
    bool _isHurting = false;
    public void Hurt()
    {
        //add force
        //rigid.AddForce(transform.right * dash_force * 0.5f);
        PlayAniamtion("Hurt");
        CameraControl.CameraShake(0.25f, 1);
    }
    //hit to sky
    public void Hurt_Fly()
    {
        const float _dashForceMultipier = 100;
        //add force to sky 
        Vector2 _dir = new Vector2(transform.right.x * 0.5f, 1);
        rigid.AddForce(_dir * dashForce * _dashForceMultipier);

        PlayAniamtion("Hurt Falling");
    }

    void Die()
    {
        Debug.Log("玩家死亡");
        SFXManager.instance.PlaySoundInstance(SFXManager.EXPLODE);
        Effect("die disappear", "die disappear");

        //disable control
        hitable.isHitable = false;
        actionController.StopAllCoroutines();

        PlayAniamtion("Die");
        this.enabled = false;
        OnDestory?.Invoke(dataIndex);

        Destroy(actionController);
        Destroy(gameObject);
    }
    //create new player 3sec after die
    void RecreatePlayer()
    {
        LocalRoomManager.instance.Revive(dataIndex);
        //Destroy(gameObject);
    }

    void ResetJumpCount()
    {
        Debug.Log("jump: TouchGround");
        jumpCount = 0;
        actionController.AddAction(jump_end);
        //animator.Play("Idle");
    }

    public void PlayAniamtion(string _clipName)
    {
        head.PlayAnimation(_clipName);
        body.PlayAnimation(_clipName);
    }
}
