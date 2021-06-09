using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateOnlineProperty : MonoBehaviour
{
    private void Awake()
    {
        LocalPlayerProperty.OnDataUpdate += UpdateOnlineData;
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        LocalPlayerProperty.OnDataUpdate -= UpdateOnlineData;
    }
    private void UpdateOnlineData(string _key, object _data, Player _player)
    {
        if (_player != null)
        {
            Debug.Log("Photon " + _player.NickName + " update " + _key);
            _player.SetCustomProperties(MyExtension.WrapToHash(new object[]
                                                               { _key, _data }));
        }
    }
}
