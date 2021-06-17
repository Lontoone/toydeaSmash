using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public static MapControl instance;
    private const string MAP_DATA_PATH = "Prefab/Maps/";
    [SerializeField]
    private Collider2D mapBounds;

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
        LoadMapPrefab();
    }
    public Vector2 viewWorldCenter
    {
        get { return Camera.main.ViewportToWorldPoint(new Vector2(0.5f, 0.5f)); }
    }
    public Vector2 GetMoveablePosition(Vector2 _in)
    {
        Vector2 _out = new Vector2(
            Mathf.Clamp(_in.x, mapBounds.bounds.min.x, mapBounds.bounds.max.x),
            Mathf.Clamp(_in.y, mapBounds.bounds.min.y, mapBounds.bounds.max.y)
            );

        return _out;
    }

    //load map
    public void LoadMapPrefab()
    {
        string _path = MAP_DATA_PATH + LocalRoomManager.instance.gamePlaySetting.GetValue<string>(GameplaySettingControl.MAP_OPT);
        Debug.Log(_path);
        GameObject _map = Instantiate(Resources.Load<GameObject>(_path), Vector2.zero, Quaternion.identity);
        /*
        //if online
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            //string _path = MAP_DATA_PATH + PhotonNetwork.MasterClient.CustomProperties[GameplaySettingControl.MAP_OPT];
            string _path = MAP_DATA_PATH + LocalRoomManager.instance.gamePlaySetting.GetValue<string>(GameplaySettingControl.MAP_OPT);
            GameObject _map = PhotonNetwork.Instantiate(_path, Vector2.zero, Quaternion.identity);
        }
        //if local
        else if (!PhotonNetwork.IsConnected) 
        {
            string _path = MAP_DATA_PATH + LocalRoomManager.instance.gamePlaySetting.GetValue<string>(GameplaySettingControl.MAP_OPT);
            Debug.Log(_path);
            GameObject _map = Instantiate( Resources.Load<GameObject>(_path),Vector2.zero,Quaternion.identity);
            
        }*/
    }

}
