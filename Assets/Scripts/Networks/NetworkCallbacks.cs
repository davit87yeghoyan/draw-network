using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Networks
{
    
    [DisallowMultipleComponent]
    public class NetworkCallbacks:MonoBehaviourPunCallbacks
    {

        public NetworkManager networkManager;
        
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }
        

        public override void OnDisconnected(DisconnectCause cause)
        {
            
            Debug.Log("OnDisconnected"+cause);
            switch (cause){
                case DisconnectCause.MaxCcuReached:
                    throw  new Exception("Ccu limited");
            }

            if (cause != DisconnectCause.DisconnectByClientLogic){
                Debug.Log("Disconnected Error: "+cause);
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("OnJoinedLobby");
            networkManager.OnJoinLobby();
        }

        public override void OnLeftLobby()
        {
            Debug.Log("OnLeftLobby");
            networkManager.OnLeftLobby();
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom");
            networkManager.OnJoinRoom();
           
        }

        public override void OnLeftRoom()
        {
            Debug.Log("OnLeftRoom");
            networkManager.OnLeftRoom();
        }

        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            StartCoroutine(networkManager.AsyncConnect());
            throw new Exception(message);
        }
        
    }
}