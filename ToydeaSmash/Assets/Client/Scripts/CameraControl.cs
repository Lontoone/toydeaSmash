using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public int maxSize = 20, minSize = 10;
    public float speed = 20;

    float _min_camera_width, _max_camera_width;
    public float player_width_screen_rate = 0.8f;

    Camera camera;

    PlayerControl[] players;
    //TODO: Player off camera warming;
    public IEnumerator Start()
    {

        //wait for all player
        yield return new WaitForFixedUpdate();

        players = FindObjectsOfType<PlayerControl>();

        //scale between all players

    }
    public void Awake()
    {
        camera = Camera.main;

        //get min camera size
        camera.orthographicSize = minSize;
        _min_camera_width = Vector2.Distance(camera.ViewportToWorldPoint(Vector2.one), camera.ViewportToWorldPoint(Vector2.zero));
        //get max camera size
        camera.orthographicSize = maxSize;
        _max_camera_width = Vector2.Distance(camera.ViewportToWorldPoint(Vector2.one), camera.ViewportToWorldPoint(Vector2.zero));
    }

    public void FixedUpdate()
    {
        //get camera bounds

        //calculate distance between players
        Vector2[] min_max = GetMinMaxPlayerPos();
        Vector3 _center = (min_max[1] - min_max[0]) * 0.5f + min_max[0];
        _center.z = -10;

        //move to the center of players
        transform.position = Vector3.Lerp(transform.position, _center, Time.fixedDeltaTime * speed);
        Debug.Log(_center);

        //get screen size
        float _screen_size = Vector2.Distance(min_max[1], min_max[0]) / player_width_screen_rate;

        //screen size to the scale of camera size
        float _camera_size = _screen_size * minSize / _min_camera_width;
        camera.orthographicSize = Mathf.Clamp(_camera_size, minSize, maxSize);


    }

    Vector2[] GetMinMaxPlayerPos()
    {
        Vector2 _min = players[0].transform.position;
        Vector2 _max = players[players.Length - 1].transform.position;
        float min_dis = 99;
        float max_dis = 0;
        for (int i = 1; i < players.Length; i++)
        {
            if (_max.magnitude < players[i].transform.position.magnitude)
            {
                _max = players[i].transform.position;
            }
            if (_min.magnitude > players[i].transform.position.magnitude)
            {
                _min= players[i].transform.position;
            }

        }

        return new Vector2[] { _min, _max };
    }
}
