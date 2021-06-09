using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalRoomStartUp : MonoBehaviour
{
    //Auto add player to local Scene:
    void Start()
    {
        LocalRoomManager.instance.AddLocalPlayer();
    }

}
