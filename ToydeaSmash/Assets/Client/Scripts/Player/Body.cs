using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    //TODO: attack action


    public static Body LoadBody(string _body_name)
    {
        return Resources.Load<Body>("Prefab/Body/" + _body_name);
    }
}
