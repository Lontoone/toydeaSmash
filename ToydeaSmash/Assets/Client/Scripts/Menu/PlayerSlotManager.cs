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
    public GameObject[] addPlayerBtns;
    public int maxSlotCount = 4;
    int _current_player_count = 0;
    public void Awake()
    {
        for (int i = 0; i < addPlayerBtns.Length; i++)
        {
            addPlayerBtns[i].SetActive(false);
        }
        for (int i = 1; i < maxSlotCount; i++)
        {
            addPlayerBtns[i].SetActive(true);
        }
        LocalRoomManager.instance.OnLocalPlayerAdded += GenerateSlot;
        LocalRoomManager.instance.OnOnlinePlayerAdded += GenerateOnlineSlot;
    }
    public void OnDestroy()
    {
        LocalRoomManager.instance.OnLocalPlayerAdded -= GenerateSlot;
        LocalRoomManager.instance.OnOnlinePlayerAdded -= GenerateOnlineSlot;
    }

    //When player enter. Add player slot
    public void GenerateSlot(LocalPlayerProperty _data)
    {

        PlayerSlot _slot = Instantiate(slot_prefab, slot_pos[_current_player_count].position, Quaternion.identity);

        _slot.SetUpPlayer(_current_player_count);

        _current_player_count++;
    }
    public void GenerateOnlineSlot(LocalPlayerProperty _data)
    {
        int __playerIndex = _data.GetValue<int>(CustomPropertyCode.PLAYER_INDEX);
        Debug.Log("GenerateOnlineSlot _player index "+ __playerIndex);
        const string _PLAYER_SLOT_PATH = "Prefab/UI/PlayerSlotOnline";
        PlayerSlot _slot = Instantiate(Resources.Load<PlayerSlot>(_PLAYER_SLOT_PATH), slot_pos[__playerIndex].position, Quaternion.identity);

        _slot.SetUpPlayer(_data.GetValue<Player>("Player"), __playerIndex);
    }
    public void CancelAllPlayerReady()
    {
        ReadyButton.CancelAllPlayerReady();
    }
    public void EnableAddPlayerBtn(bool _isActive)
    {
        for (int i = _current_player_count; i < maxSlotCount; i++)
        {
            addPlayerBtns[i].SetActive(_isActive);
        }
    }
}
