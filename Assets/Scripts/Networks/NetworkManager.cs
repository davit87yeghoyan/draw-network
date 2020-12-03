using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Networks
{
    [RequireComponent(typeof(NetworkCallbacks))]
    [DisallowMultipleComponent]
    public class NetworkManager : MonoBehaviour
    {

        public static NetworkManager Instance;
        
        public event Action LeftLobby;
        public event Action JoinLobby;
        public event Action JoinRoom;
        public event Action LeftRoom;


        

        private void Start()
        {
            Connect();
            JoinLobby += CreateRoom;
        }

        private void OnEnable()
        {
            Instance = this;
        }


        private void OnDisable()
        {
            Instance = null;
        }

        
        public void Connect()
        {
            
            PhotonNetwork.GameVersion = Application.version;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }


        private void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions {PublishUserId = true, MaxPlayers = byte.MaxValue};
            PhotonNetwork.JoinOrCreateRoom("roomName1",roomOptions, TypedLobby.Default);
        }

        public IEnumerator AsyncConnect()
        {
            yield return new WaitForSeconds(2f);
            Connect();
        }


        public void Disconnect()
        {
            PhotonNetwork.Disconnect();
        }
        
        
        public static void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        
        public void OnLeftLobby()
        {
            LeftLobby?.Invoke();
        }

        public void OnJoinLobby()
        {
            JoinLobby?.Invoke();
        }


        public void  OnJoinRoom()
        {
            JoinRoom?.Invoke();
        }

        public void OnLeftRoom()
        {
            LeftRoom?.Invoke();
        }
    }
}