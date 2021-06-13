using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PhysicsControlListeners : MonoBehaviour
{
    public LayerMask ground_layer;
    public GameObject footPositon; //腳步位置
    //public Vector2 move_dir; //移動方向
    public bool isGrounded;
    public bool isWalled;
    public float touch_ground_radious = 0.05f;
    public event Action eOnTouchGround;
    public float side_bounces_force = 100; //force to add while hit wall (not ground.)
    public GameObject currentStandingGround
    {
        get
        {
            RaycastHit2D hit = Physics2D.Raycast(footPositon.transform.position, -transform.up, touch_ground_radious, ground_layer);
            if (hit.collider != null)
            {
                return hit.collider.gameObject;
            }
            else
            {
                return null;
            }
        }
    }

    private bool last_frame_isGrounded;//上一次更新是否碰到地面
    private Rigidbody2D rigidbody;

    void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();

        if (footPositon == null)
        {
            //產生腳步位置標記物件:
            GameObject temp = new GameObject();
            footPositon = Instantiate(temp, transform.position, Quaternion.identity, transform);
            footPositon.gameObject.name = "foot";
            Destroy(temp);
        }

    }

    private void FixedUpdate()
    {
        //離開/碰地事件:
        if (last_frame_isGrounded == !isGrounded)
        {
            if (eOnTouchGround != null && isGrounded)
                eOnTouchGround();
            last_frame_isGrounded = isGrounded;
        }
        //碰地面偵測
        //isGrounded = Physics2D.OverlapCircle(footPositon.transform.position, touch_ground_radious, ground_layer);
        isGrounded = Physics2D.Raycast(footPositon.transform.position, -transform.up, touch_ground_radious, ground_layer);
        //isWalled = WallHitDetect();
    }
    private void OnDrawGizmos()
    {
        if (footPositon != null)
            Gizmos.DrawWireSphere(footPositon.transform.position, touch_ground_radious);
    }

    bool WallHitDetect()
    {
        if (Physics2D.Raycast(footPositon.transform.position, -transform.right, touch_ground_radious, ground_layer) ||
            Physics2D.Raycast(footPositon.transform.position, transform.right, touch_ground_radious, ground_layer))
        {
            //rigidbody.AddForce(transform.right * side_bounces_force * rigidbody.velocity.normalized);
            rigidbody.AddForce(transform.right * rigidbody.velocity * rigidbody.mass);
            Debug.Log("wall anti v" + rigidbody.velocity);
            return true;
        }
        return false;
    }
}
