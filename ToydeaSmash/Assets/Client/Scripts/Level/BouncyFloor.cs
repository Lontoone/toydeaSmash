using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BouncyFloor : MonoBehaviour
{
    public float force = 700;
    public float shackStrength = 20;
    public float duration = 0.2f;
    private Coroutine c_bouncyGap;
    private WaitForSeconds _wait;

    private void Start()
    {
        _wait = new WaitForSeconds(duration);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (c_bouncyGap == null)
        {
            c_bouncyGap = StartCoroutine(BounceCoro(collision.gameObject));
        }

    }

    IEnumerator BounceCoro(GameObject _obj)
    {
        _obj.GetComponent<Rigidbody2D>()?.AddForce(transform.up * force);
        transform.DOShakeScale(0.2f, shackStrength, 100);
        yield return _wait;
        c_bouncyGap = null;
    }

}
