using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public int maxSize = 20, minSize = 10;
    public float speed = 20;
    Camera camera;

    PlayerControl[] players;
    //TODO: Player off camera warming;
    public IEnumerator Start()
    {
        camera = Camera.main;

        //wait for all player
        yield return new WaitForFixedUpdate();

        players = FindObjectsOfType<PlayerControl>();

        //scale between all players

    }

    public void FixedUpdate()
    {
        //get camera bounds

        //calculate distance between players
        Vector2[] min_max = GetMinMaxPlayerPos();
        Vector2 _center = (min_max[1] - min_max[0]) * 0.5f + min_max[0];

        //move to the center of players
        transform.position = Vector2.Lerp(transform.position, _center, Time.fixedDeltaTime * speed);

    }

    Vector2[] GetMinMaxPlayerPos()
    {
        Vector2 _min = players[0].transform.position;
        Vector2 _max = players[0].transform.position;
        float min_dis = 99;
        float max_dis = 0;
        for (int i = 1; i < players.Length; i++)
        {
            float _dis = Vector2.Distance(players[i].transform.position, _min);
            //get smallest
            if (_dis < min_dis)
            {
                _min = players[i].transform.position;
                min_dis = _dis;
            }

            //get biggest
            _dis = Vector2.Distance(players[i].transform.position, _max);
            if (_dis > max_dis)
            {
                _max = players[i].transform.position;
                max_dis = _dis;
            }

        }

        return new Vector2[] { _min, _max };
    }
}
