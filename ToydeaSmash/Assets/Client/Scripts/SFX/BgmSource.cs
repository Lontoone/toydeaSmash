using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmSource : MonoBehaviour
{
    public static BgmSource instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


}
