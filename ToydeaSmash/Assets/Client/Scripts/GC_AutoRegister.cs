using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GC_AutoRegister : MonoBehaviour
{
    public string KEY = "";
    public void Awake()
    {
        GCManager.RegisterObject(KEY, gameObject);
    }
}
