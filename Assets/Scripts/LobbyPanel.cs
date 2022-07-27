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

    public GameObject RoomListPanel;
    public GameObject RoomListEntryPrefeb;

    [Header("Create Room Panel")]
    public GameObject CreateRoomPanel;

    public TMP_InputField RoomNameInput;
    public TMP_InputField MaxPlayersInput;

    [Header("InRoom Panel")]
    public GameObject InRoomPanel;

    public GameObject PlayerListPanel;
    public GameObject PlayerListEntryPrefeb;
    public GameObject ChatPanel;
    public GameObject StatePanel;
    public GameObject MapPanel;
    public TMP_InputField ChatInput;
    public Button ReadyStartButton;

    [Header("Loading Panel")]
    public GameObject LoadingPanel;

    #endregion

    #region LOBBY PARAM

    public bool isOffline = false;

    public string playerName;
    
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
            // Å¬¸®¾î ·ë ¸®½ºÆ® ºä
        }
    }

    public override void OnLeftLobby()
    {
        if (isOffline == false)
        {
            cachedRoomList.Clear();
            // Å¬¸®¾î ·ë ¸®½ºÆ® ºä
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Å¬¸®¾î ·ë ¸®½ºÆ® ºä
        // ¾÷µ¥ÀÌÆ® ·ë ¸®½ºÆ® Ä³½¬
        // ¾÷µ¥ÀÌÆ® ·ë ¸®½ºÆ®ºä
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
        }
    }

    public override void OnLeftRoom()
    {
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

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(null);
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

    public void OnReadyStartButtonClicked()
    {
        // all ready state
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {

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
