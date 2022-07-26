using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class LobbyPanel : MonoBehaviourPunCallbacks
{
    #region UI PARAM

    public InputField PlayerNameInput;

    #endregion

    #region LOBBY PARAM

    public bool isOffline = true;
    public string playerName;
    public string sceneName = "LookA_Scene";

    #endregion

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        
    }

    #endregion

    #region UI CALLBACKS

    public void OnLoginButtonClicked()
    {
        isOffline = false;
    }
    public void OnOfflineButtonClicked()
    {
        isOffline = true;
    }

    public void OnCreateRoomButtonClicked()
    {

    }
    public void OnJoinRandomRoomButtonClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnBackButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnReadyButtonClicked()
    {

    }

    public void OnStartButtonClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        PhotonNetwork.LoadLevel(sceneName);
    }

    #endregion

}
