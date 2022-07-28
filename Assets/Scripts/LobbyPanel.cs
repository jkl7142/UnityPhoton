using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class LobbyPanel : MonoBehaviourPunCallbacks
{
    #region UI PARAM

    [Header("Login Panel")]
    public GameObject LoginPanel;

    public TMP_InputField PlayerNameInput;

    [Header("Selection Panel")]
    public GameObject SelectionPanel;

    public GameObject RoomListScrollView;
    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public TMP_InputField RoomNameInput;
    public TMP_InputField MaxPlayersInput;

    [Header("InRoom Panel")]
    public GameObject InRoomPanel;

    public GameObject PlayerListPanel;
    public GameObject PlayerListEntryPrefab;
    public GameObject ChatPanel;
    public GameObject StatePanel;
    public GameObject MapPanel;
    public TMP_InputField ChatInput;
    public Button StartButton;

    [Header("Loading Panel")]
    public GameObject LoadingPanel;

    #endregion

    #region LOBBY PARAM

    public bool isOffline = false;

    public string playerName;
    public int playerNumber = 1;
    
    public string roomName;
    public byte maxPlayers = 16;
    public RoomOptions roomOptions;

    private string sceneName = "SampleScene";

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    #endregion

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        //Screen.SetResolution(960, 540, false);

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        SetActivePanel(LoginPanel.name);
    }

    #region PUN CALLBACKS

    public void OnConnect()
    {
        if (isOffline)
        {
            PhotonNetwork.Disconnect();

            PhotonNetwork.OfflineMode = isOffline;
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            SetActivePanel(SelectionPanel.name);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());
        if (isOffline)
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(LoginPanel.name);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());

        if (isOffline == false)
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }
    }

    public override void OnLeftLobby()
    {
        if (isOffline == false)
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(SelectionPanel.name);
    }

    //
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        string roomName = "Room " + UnityEngine.Random.Range(1000, 10000);

        RoomOptions options = new RoomOptions { MaxPlayers = 16 };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());
        if (isOffline)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            cachedRoomList.Clear();

            SetActivePanel(InRoomPanel.name);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }
            
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(PlayerListPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Init(p.ActorNumber, p.NickName);
                
                playerListEntries.Add(p.ActorNumber, entry);
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                StartButton.gameObject.SetActive(true);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());
        
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(PlayerListPanel.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListEntry>().Init(newPlayer.ActorNumber, newPlayer.NickName);
        
        playerListEntries.Add(newPlayer.ActorNumber, entry);

        // StartButton.gameObject.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        // StartButton.gameObject.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        if (playerListEntries.Count == 0)
            return;

        foreach (GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnCreatedRoom()
    {
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartButton.gameObject.SetActive(true);
        }
    }

    #endregion

    #region UI CALLBACKS

    public void OnLoginButtonClicked()
    {
        isOffline = false;
        playerName = PlayerNameInput.text;

        if (playerName == "" || playerName.Length > 12)
        {
            PlayerNameInput.text = "";
            PlayerNameInput.placeholder.GetComponent<TMP_Text>().text = " Please Name(Length 1 ~ 12)";
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            OnConnect();
        }
    }

    public void OnOfflineButtonClicked()
    {

        isOffline = true;
        OnConnect();
    }

    public void OnCreateRoomButtonClicked()
    {
        SetActivePanel(CreateRoomPanel.name);
    }

    public void OnJoinRandomRoomButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.Disconnect();
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());
    }

    public void OnCreateButtonClicked()
    {
        if (RoomNameInput.text == "" || RoomNameInput.text.Length > 20)
        {
            RoomNameInput.text = "";
            RoomNameInput.placeholder.GetComponent<TMP_Text>().text = " Please Name(Length 1 ~ 20)";
        }
        else if (Int32.Parse(MaxPlayersInput.text) <= 0 || Int32.Parse(MaxPlayersInput.text) > 16 || IsDigit(MaxPlayersInput.text) == false)
        {
            MaxPlayersInput.text = "";
            MaxPlayersInput.placeholder.GetComponent<TMP_Text>().text = " Please Number(limit 1 ~ 16)";
        }
        else
        {
            roomName = RoomNameInput.text;
            maxPlayers = Byte.Parse(MaxPlayersInput.text);
            roomOptions = new RoomOptions{ MaxPlayers = maxPlayers };

            PhotonNetwork.CreateRoom(roomName, roomOptions);
            SetActivePanel(InRoomPanel.name);
        }
    }

    public void OnCancelButtonClicked()
    {
        SetActivePanel(SelectionPanel.name);
    }

    public void OnStartButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(sceneName);
        }
    }

    public void OnLeaveButtonClicked()
    {
        SetActivePanel(SelectionPanel.name);
        PhotonNetwork.LeaveRoom();
    }

    private void SetActivePanel(string activePanel)
    {
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        InRoomPanel.SetActive(activePanel.Equals(InRoomPanel.name));
        LoadingPanel.SetActive(activePanel.Equals(LoadingPanel.name));
    }

    #endregion

    private void ClearRoomListView()
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntry>().Init(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

            roomListEntries.Add(info.Name, entry);
        }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    public bool IsDigit(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] < 48 || str[i] > 57)
            {
                return false;
            }
        }

        return true;
    }
}
