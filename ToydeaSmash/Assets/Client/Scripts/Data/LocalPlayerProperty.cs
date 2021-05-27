using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerProperty
{
    public Dictionary<string, object> playerProperty = new Dictionary<string, object>();

    public void SetProperty(string _key, object _data)
    {
        if (playerProperty.ContainsKey(_key))
        {
            playerProperty[_key] = _data;
            Debug.Log("set "+ _key+" "+_data);
        }
        else
        {
            playerProperty.Add(_key, _data);
            Debug.Log("add " + _key + " " + _data);
        }
        
    }
}
