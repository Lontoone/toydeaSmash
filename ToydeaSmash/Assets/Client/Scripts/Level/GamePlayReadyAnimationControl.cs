using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayReadyAnimationControl : MonoBehaviour
{
    public float startDelay =0.5f;
    public float lastDuration = 7;
    private Animator animator;
    private SpriteRenderer _sp;
    private IEnumerator Start()
    {
        _sp = GetComponent<SpriteRenderer>();
        _sp.enabled = false;
        animator = GetComponent<Animator>();

        yield return new WaitForSeconds(startDelay);
        _sp.enabled = true;

        animator.Play("ready animation");
        yield return new WaitForSeconds(lastDuration);
        Destroy(gameObject);
    }

}
