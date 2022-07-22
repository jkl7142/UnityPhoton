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
    string roomName = "SampleScene";
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
        DebugText.text = "룸 접속 시도 중";
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
            // 마스터 서버에 접속되면 바로 룸 접속 시도
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        DebugText.text = "마스터 서버에 접속 됨";
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DebugText.text = "마스터 서버에 접속 재시도 중";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        DebugText.text = "새로운 룸 생성";
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        DebugText.text = "룸 접속 성공";
        //PhotonNetwork.LoadLevel(roomName);
        PhotonNetwork.Instantiate("player", Vector3.zero, Quaternion.identity);
        Debug.Log("clone player");
    }
}
