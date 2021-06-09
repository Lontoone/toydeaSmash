using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class LocalPlayerProperty
{
    public static event Action<string, object, Player> OnDataUpdate;
    public Dictionary<string, object> playerProperty = new Dictionary<string, object>();


    public void SetProperty(string _key, object _data)
    {
        Player _playerData = GetValue<Player>("Player");

        if (playerProperty.ContainsKey(_key))
        {
            playerProperty[_key] = _data;
            Debug.Log("set " + _key + " " + _data);
        }
        else
        {
            playerProperty.Add(_key, _data);
            Debug.Log("add " + _key + " " + _data);
        }
        OnDataUpdate?.Invoke(_key, _data, _playerData);
    }

    public T GetValue<T>(string _key)
    {
        object data;
        if (playerProperty.TryGetValue(_key, out data))
        {
            return (T)data;
        }
        else
        {
            return default;
        }
    }


}
