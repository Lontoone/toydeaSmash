using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager instance;
    public const string DEATH = "DEATH";
    public const string KILL = "KILL";
    public const string DAMAGE = "DAMAGE";
    public const string DAMAGETAKE = "DAMAGETAKE";

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

    public void Start()
    {
        HitableObj.OnKilled += AddKillAndDeathCount;
        HitableObj.Hit_event += AddDamageCount;
    }
    public void OnDestroy()
    {
        HitableObj.OnKilled -= AddKillAndDeathCount;
        HitableObj.Hit_event -= AddDamageCount;
    }

    void AddKillAndDeathCount(GameObject target, GameObject killer)
    {
        SetProperty(killer.GetComponent<PlayerControl>(), KILL, 1);
        SetProperty(target.GetComponent<PlayerControl>(), DEATH, 1);
    }
    void AddDamageCount(GameObject t, float d, GameObject s)
    {
        SetProperty(t.GetComponent<PlayerControl>(), DAMAGE, (int)d);
        SetProperty(s.GetComponent<PlayerControl>(), DAMAGETAKE, (int)d);
    }


    void SetProperty(PlayerControl player, string _key, int data_to_add)
    {
        if (player != null)
        {
            object _outdata;
            if (LocalRoomManager.instance.players[player.dataIndex].playerProperty.TryGetValue(_key, out _outdata))
            {
                LocalRoomManager.instance.players[player.dataIndex].SetProperty(_key,
                    (int)_outdata + data_to_add);
            }
            else
            {
                //init value
                LocalRoomManager.instance.players[player.dataIndex].SetProperty(_key, 0);
            }

        }
    }
}
