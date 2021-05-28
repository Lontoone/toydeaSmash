using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public int maxSize = 10, minSize = 5;
    Camera camera;

    public IEnumerator Start()
    {
        camera = Camera.main;

        //wait for all player
        yield return new WaitForFixedUpdate();

        //scale between all players
        
    }

    public void FixedUpdate()
    {
        //get camera bounds

        //calculate distance between players

        //move to the center of players

    }
}
