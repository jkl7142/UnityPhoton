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
        DebugText.text = "�� ���� �õ� ��";
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
            // ������ ������ ���ӵǸ� �ٷ� �� ���� �õ�
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        DebugText.text = "������ ������ ���� ��";
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DebugText.text = "������ ������ ���� ��õ� ��";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        DebugText.text = "���ο� �� ����";
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        DebugText.text = "�� ���� ����";
        //PhotonNetwork.LoadLevel(roomName);
        PhotonNetwork.Instantiate("player", Vector3.zero, Quaternion.identity);
        Debug.Log("clone player");
    }
}
