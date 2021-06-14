using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCameraWhenHit : MonoBehaviour
{
    public float duration = 0.5f;
    public float strength = 2.5f;
    private HitableObj _hitableObj;

    public void Start()
    {
        _hitableObj = GetComponent<HitableObj>();
        _hitableObj.gotHit_event += ShakeCamera;
    }
    public void OnDestroy()
    {
        _hitableObj.gotHit_event -= ShakeCamera;
    }
    private void ShakeCamera()
    {
        CameraControl.CameraShake(duration, strength);
    }
}
