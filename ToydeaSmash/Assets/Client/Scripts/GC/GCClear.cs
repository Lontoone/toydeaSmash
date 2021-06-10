using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GCClear : MonoBehaviour
{
    public void Start()
    {
        GCManager.Clear();
    }
}
