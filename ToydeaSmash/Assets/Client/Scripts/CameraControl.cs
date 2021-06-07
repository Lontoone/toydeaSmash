using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public int maxSize = 20, minSize = 10;
    public float speed = 20;
    public float player_width_screen_rate = 0.5f;



    private static float s_min_camera_width, s_max_camera_width;
    private static PlayerControl[] s_players;
    private static Camera s_camera;
    public IEnumerator Start()
    {
        //wait for all player
        yield return new WaitForFixedUpdate();
        GetPlayerList(0);

        //scale between all players
        PlayerControl.OnCreate += GetPlayerList;
        PlayerControl.OnDestory += GetPlayerList;
    }
    public void OnDestroy()
    {
        PlayerControl.OnCreate -= GetPlayerList;
        PlayerControl.OnDestory -= GetPlayerList;
    }

    void GetPlayerList(int player_index)
    {
        s_players = FindObjectsOfType<PlayerControl>();
    }

    public void Awake()
    {
        s_camera = Camera.main;

        //get min camera size
        s_camera.orthographicSize = minSize;
        s_min_camera_width = Vector2.Distance(s_camera.ViewportToWorldPoint(Vector2.one), s_camera.ViewportToWorldPoint(Vector2.zero));
        //get max camera size
        s_camera.orthographicSize = maxSize;
        s_max_camera_width = Vector2.Distance(s_camera.ViewportToWorldPoint(Vector2.one), s_camera.ViewportToWorldPoint(Vector2.zero));
    }

    public void FixedUpdate()
    {
     
        //calculate distance between players
        Vector2[] min_max = GetMinMaxPlayerPos();
        Vector3 _center = (min_max[1] - min_max[0]) * 0.5f + min_max[0];
        _center.z = -10;

        //move to the center of players
        transform.position = Vector3.Lerp(transform.position, _center, Time.fixedDeltaTime * speed);

        //get screen size
        float _screen_size = Vector2.Distance(min_max[1], min_max[0]) / player_width_screen_rate;

        //screen size to the scale of camera size
        float _camera_size = _screen_size * minSize / s_min_camera_width;
        s_camera.orthographicSize = Mathf.Clamp(_camera_size, minSize, maxSize);


    }

    Vector2[] GetMinMaxPlayerPos()
    {
        Vector2 _min = s_players[0].transform.position;
        Vector2 _max = s_players[0].transform.position;
        if (s_players == null || s_players.Length <= 0)
        {
            return new Vector2[] { Vector2.zero, Vector2.zero };
        }

        if (s_players.Length < 2)
        {
            return new Vector2[] { _min, _max };
        }
        else
        {
            for (int i = 0; i < s_players.Length; i++)
            {
                if (s_players[i] == null)
                    continue;
                if (_max.magnitude < s_players[i].transform.position.magnitude)
                {
                    _max = s_players[i].transform.position;
                }
                if (_min.magnitude > s_players[i].transform.position.magnitude)
                {
                    _min = s_players[i].transform.position;
                }

            }
        }
        return new Vector2[] { _min, _max };
    }
}
