using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BouncyFloor : MonoBehaviour
{
    public float force = 700;
    public float shackStrength = 20;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<Rigidbody2D>()?.AddForce(transform.up * force);
        transform.DOShakeScale(0.2f, shackStrength, 100);
    }

}
