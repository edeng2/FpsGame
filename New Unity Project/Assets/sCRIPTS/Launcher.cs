﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;


    public static RoomInfo currentRoomInfo;
    [SerializeField] TMP_InputField roomNameIF;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] RoomManager roomManagerPrefab;
    public static bool inRoom = false;
    // Start is called before the first frame update

    void Awake()
    {
        if (instance)///////////////////
        {
            
            return;
        }
        instance = this;
    }
    void Start()
    {
       
        PhotonNetwork.ConnectUsingSettings();
        CreateRoomManager();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("Title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
        
    }

   public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameIF.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameIF.text);
        
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        inRoom = true;
        MenuManager.instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Joined Room "+ PhotonNetwork.CurrentRoom.Name);

        Player[] players = PhotonNetwork.PlayerList;
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.instance.OpenMenu("Error");
    }

    public void StartGame()////////////////////////////////////////////
    {


        if (RoomManager.Instance.AllPlayersReady()) {
            Debug.Log("All players ready");
           
            PhotonNetwork.LoadLevel(1);
        }
            
       
        
    }
   
    
   

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("Loading");
        currentRoomInfo = info;
        
    }

    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("Title");
        inRoom = false;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }
       foreach(RoomInfo r in roomList)
        {
            if (r.RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(r);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void CreateRoomManager()
    {
        if(!FindObjectOfType<RoomManager>())
            Instantiate(roomManagerPrefab).name="RoomManager";
    }

   

    
}
