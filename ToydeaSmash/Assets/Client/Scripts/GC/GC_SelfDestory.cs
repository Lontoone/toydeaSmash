using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GC_SelfDestory : MonoBehaviour
{
    public string key;
    public float lifeTime = 1;
    public void OnEnable()
    {
        Invoke("DoCollect",lifeTime);
    }

    void DoCollect()
    {
        Debug.Log("DoCollect");
        GCManager.Destory(key, gameObject);
    }

}
