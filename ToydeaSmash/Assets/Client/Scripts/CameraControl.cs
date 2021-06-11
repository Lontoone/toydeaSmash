using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraControl : MonoBehaviour
{
    public int maxSize = 20, minSize = 10;
    public float speed = 20;
    public float player_width_screen_rate = 0.5f;
    public Vector2 centerOffset;
    public float threshold = 0.01f;


    private static float s_min_camera_width, s_max_camera_width;
    private PlayerControl[] s_players;
    private static Camera s_camera;
    private static Coroutine s_c_checkPlayersInSight;
    public IEnumerator Start()
    {
        //wait for all player
        yield return new WaitForFixedUpdate();
        GetPlayerList(0);

        //scale between all players
        PlayerControl.OnCreate += GetPlayerList;
        PlayerControl.OnDestory += GetPlayerList;

        s_c_checkPlayersInSight = StartCoroutine(CheckAllPlayersInSideSightCoro());
    }
    public void OnDestroy()
    {
        PlayerControl.OnCreate -= GetPlayerList;
        PlayerControl.OnDestory -= GetPlayerList;
        StopCoroutine(s_c_checkPlayersInSight);
        s_c_checkPlayersInSight = null;
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
        Vector3 _center = (min_max[1] - min_max[0]) * 0.5f + min_max[0] + centerOffset;
        _center.z = -10;

        //move to the center of players
        transform.position = Vector3.Lerp(transform.position, _center, Time.fixedDeltaTime * speed);

        //get screen size
        float _screen_size = Vector2.Distance(min_max[1], min_max[0]) / player_width_screen_rate;

        //screen size to the scale of camera size

        float _camera_size = _screen_size * minSize / s_min_camera_width;

        if (Mathf.Abs(_camera_size - s_camera.orthographicSize) > threshold)
            s_camera.orthographicSize = Mathf.Lerp(s_camera.orthographicSize, Mathf.Clamp(_camera_size, minSize, maxSize), Time.fixedDeltaTime * speed);
    }

    Vector2[] GetMinMaxPlayerPos()
    {
        if (s_players == null || s_players.Length < 0)
        {
            return new Vector2[] { Vector2.zero, Vector2.zero };
        }
      
        Vector2 _min = s_players[0].transform.position;
        Vector2 _max = s_players[0].transform.position;
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

    private IEnumerator CheckAllPlayersInSideSightCoro()
    {
        WaitForSeconds _presec = new WaitForSeconds(1);
        while (true)
        {
            CheckAllPlayersInsideSight();
            yield return _presec;
        }
        s_c_checkPlayersInSight = null;

    }

    //Damage 1% of life pre sec if a player is out side the view port
    private void CheckAllPlayersInsideSight()
    {
        //loop all the players
        for (int i = 0; i < s_players.Length; i++)
        {
            if (s_players[i] == null)
            {
                return;
            }
            //check player's viewport position.
            Vector2 __playerViewPortPosition = s_camera.WorldToViewportPoint(s_players[i].transform.position);
            if (__playerViewPortPosition.x < 0 || __playerViewPortPosition.x > 1)
            {
                //TODO: temp using null as soruces and minus 100 pre sec
                HitableObj.Hit_event_c(s_players[i].gameObject, 100, null);
            }
        }
    }

    public static void CameraShake(float _duration, float _strength)
    {
        s_camera.transform.DOShakePosition(_duration, _strength);
    }
}
