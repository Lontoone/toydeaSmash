using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GamePlayReadyAnimationControl : MonoBehaviour
{
    public float startDelay =0.5f;
    public float lastDuration = 7;
    public UnityEvent OnGameStart;
    public UnityEvent OnAnimationEnd;
    private Animator animator;
    private SpriteRenderer _sp;
    private IEnumerator Start()
    {

        _sp = GetComponent<SpriteRenderer>();
        _sp.enabled = false;
        animator = GetComponent<Animator>();

        OnGameStart?.Invoke();

        yield return new WaitForSeconds(startDelay);
        _sp.enabled = true;

        animator.Play("ready animation");
        yield return new WaitForSeconds(lastDuration);
        OnAnimationEnd?.Invoke();

        Destroy(gameObject);
    }

}
