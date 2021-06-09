using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{

    public static Launcher instance;
    [SerializeField] UnityEngine.UI.InputField roomNameInputField;
    [SerializeField] UnityEngine.UI.Text errorText;
    [SerializeField] UnityEngine.UI.Text roomNameText;
    [SerializeField] Transform roomListItemPrefab;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject StartGameButton;

    public void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //connect to server
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("connect to lobby");
        PhotonNetwork.JoinLobby();
       
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("title");
        Debug.Log("Join lobby");

        //?temp
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
    }


    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text)) { return; }
        PhotonNetwork.CreateRoom(roomNameInputField.text);

        //避免玩家在等待server時亂點其他按鈕s
        MenuManager.instance.OpenMenu("loading");
    }


    public override void OnJoinedRoom() //called when create or join a room
    {
        MenuManager.instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        //只有host可以開始遊戲按鈕
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //switch host時讓下個host可以開始遊戲
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room create Failed " + message;
        MenuManager.instance.OpenMenu("error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("loading");
    }


    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }


    //When Players all enter the room
    public void PlayerCustomMenu() {
        //PhotonNetwork.LoadLevel(1);
        PhotonNetwork.LoadLevel("Character selection_Online");
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
