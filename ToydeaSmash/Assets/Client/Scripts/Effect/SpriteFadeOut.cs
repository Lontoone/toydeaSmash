using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFadeOut : MonoBehaviour
{
    static WaitForFixedUpdate _wait = new WaitForFixedUpdate();
    public float duration;
    public Gradient gradient;
    [HideInInspector]
    public SpriteRenderer sp;
    Coroutine _cFade;
    float _step=1;
    
    public void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    public void OnEnable()
    {
        _step = Time.fixedDeltaTime / duration;
        _cFade = StartCoroutine(Fade_coro());
    }
    public void OnDisable()
    {
        StopCoroutine(_cFade);
    }

    IEnumerator Fade_coro()
    {
        //only fade alpha
        float _t = 0;
        while (_t < 1)
        {
            _t += _step;
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, gradient.Evaluate(_t).a);
            yield return _wait;
        }
    }
}
