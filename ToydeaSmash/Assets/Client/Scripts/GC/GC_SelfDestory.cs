using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GC_SelfDestory : MonoBehaviour
{
    public string key;
    public float lifeTime = 1;
    public bool unRegisterWhenDestory = true;
    public void OnEnable()
    {
        Invoke("DoCollect", lifeTime);
    }

    void DoCollect()
    {
        Debug.Log("DoCollect");
        GCManager.Destory(key, gameObject);
    }
    private void OnDestroy()
    {
        if (unRegisterWhenDestory)
        {
            GCManager.Remove(key);
        }
    }

}
