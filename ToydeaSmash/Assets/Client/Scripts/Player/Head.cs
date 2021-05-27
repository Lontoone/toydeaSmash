using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    //TODO: special skill

    public static Head LoadHead(string _head_name)
    {
        return Resources.Load<Head>("Prefab/Head/" + _head_name);
    }
}
