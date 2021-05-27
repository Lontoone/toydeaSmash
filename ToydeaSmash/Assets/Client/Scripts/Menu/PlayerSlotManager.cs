using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlotManager : MonoBehaviourPunCallbacks
{
    //public PlayerSlot[] slots;
    public PlayerSlot slot_prefab;
    public Transform[] slot_pos;
    int _current_player_count = 0;
    public void Start()
    {
        LocalRoomManager.instance.OnLocalPlayerAdded += GenerateSlot;
    }
    public void OnDestroy()
    {
        LocalRoomManager.instance.OnLocalPlayerAdded -= GenerateSlot;
    }

    //When player enter. Add player slot
    public void GenerateSlot(LocalPlayerProperty _data) {

        PlayerSlot _slot = Instantiate(slot_prefab, slot_pos[_current_player_count].position,Quaternion.identity);

        _slot.SetUpPlayer(_current_player_count);

        _current_player_count++;

    }
}
