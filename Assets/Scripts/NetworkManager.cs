using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    RoomOptions roomOptions = new RoomOptions { MaxPlayers = 16 };

    [SerializeField]
    TextMeshProUGUI DebugText;

    public static NetworkManager Instance;

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        OnConnect();
    }

    // Update is called once per frame
    void Update()
    {
        DebugText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public void OnConnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("InGame", roomOptions, null);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("player", Vector3.zero, Quaternion.identity);
        Debug.Log("clone player");
    }
}
