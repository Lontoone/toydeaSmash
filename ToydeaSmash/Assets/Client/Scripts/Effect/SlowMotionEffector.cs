using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionEffector : MonoBehaviour
{
    public static SlowMotionEffector instance;
    public float duration = 2f;
    public AnimationCurve slowDownCurve;
    public AnimationCurve recoverCurve;
    //private static
    private static Coroutine s_c_doSlowMotion;
    private static WaitForSecondsRealtime s_wait;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
        s_wait = new WaitForSecondsRealtime(Time.fixedDeltaTime);
    }
    public void DoSlowMotion()
    {
        if (s_c_doSlowMotion == null)
        {
            s_c_doSlowMotion = StartCoroutine(DoSlowMotionCoro(duration, slowDownCurve, recoverCurve));
        }
    }
    private static IEnumerator DoSlowMotionCoro(float _duration, AnimationCurve _inCurve, AnimationCurve _outCurve)
    {
        float _counter = 0;
        while (_counter < _duration)
        {
            _counter += Time.fixedUnscaledDeltaTime;
            float _eva = _counter / _duration;
            Time.timeScale = Mathf.Clamp( _inCurve.Evaluate(_eva),0,1);
            Debug.Log("slow motion "+ Time.timeScale +" in "+ _counter);
            yield return s_wait;
        }
        //back to 1
        _counter = 0;
        while (_counter < _duration)
        {
            _counter += Time.fixedUnscaledDeltaTime;
            float _eva = _counter / _duration;
            Time.timeScale = Mathf.Clamp(_outCurve.Evaluate(_eva), 0, 1);
            Debug.Log("slow motion " + Time.timeScale + " out " + _counter);
            yield return s_wait;
        }
        Time.timeScale = 1;
        s_c_doSlowMotion = null;
    }

}
