using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class RoomListEntry : MonoBehaviour
{
    public TMP_Text RoomNameText;
    public TMP_Text RoomPlayersText;
    public Button JoinRoomButton;

    private string roomName;

    public void Start()
    {
        JoinRoomButton.onClick.AddListener(() =>
        {
            PhotonNetwork.JoinRoom(roomName);
        });
    }

    public void Init(string name, byte currentPlayers, byte maxPlayers)
    {
        roomName = name;
        RoomNameText.text = name;
        RoomPlayersText.text = currentPlayers + " / " + maxPlayers;
    }
}