using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public static MapControl instance;
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
    }
    //public Transform[]
    public Vector2 viewWorldCenter
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 1)); }
    }


}
