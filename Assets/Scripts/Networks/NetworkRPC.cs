using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Networks
{
    [DisallowMultipleComponent]
    public class NetworkRPC:MonoBehaviour
    {

        public PhotonView photonView;
        public event Action Clear;
        public event Action<Vector2,Player,bool> DrawCircle;
        public event Action<Vector2,Vector2,Player,bool> DrawDrag;
        public event Action<Player,bool> InputDownStatus;
        
        
        public void RPC_Clear()
        {
            photonView.RPC(nameof(RPC_OnClear), RpcTarget.AllBuffered);
        }
        
        public void RPC_DrawCircle(Vector2 pos)
        {
            photonView.RPC(nameof(RPC_OnDrawCircle), RpcTarget.AllBuffered,pos);
        }
        
        public void RPC_DrawDrag(Vector2 from,Vector2 to)
        {
            photonView.RPC(nameof(RPC_OnDrawDrag), RpcTarget.AllBuffered,from,to);
        }        
        
        public void RPC_InputDownStatus()
        {
            photonView.RPC(nameof(RPC_OnInputDownStatus), RpcTarget.AllBuffered);
        }
        
        
        
        
        #region PunRPC events
        [PunRPC]
        private void RPC_OnClear(PhotonMessageInfo info)
        {
            PhotonNetwork.OpRemoveCompleteCache();
            
            if(Equals(info.Sender, PhotonNetwork.LocalPlayer)) return;
            Clear?.Invoke();
        }        
        
        [PunRPC]
        private void RPC_OnDrawCircle(Vector2 pos, PhotonMessageInfo info)
        {
            if(Equals(info.Sender, PhotonNetwork.LocalPlayer)) return;
            DrawCircle?.Invoke(pos,info.Sender,true);
        }
        
        [PunRPC]
        private void RPC_OnDrawDrag(Vector2 from,Vector2 to,PhotonMessageInfo info)
        {
            if(Equals(info.Sender, PhotonNetwork.LocalPlayer)) return;
            DrawDrag?.Invoke(from,to,info.Sender, true);
        }        
        
        [PunRPC]
        private void RPC_OnInputDownStatus(PhotonMessageInfo info)
        {
            if(Equals(info.Sender, PhotonNetwork.LocalPlayer)) return;
            InputDownStatus?.Invoke(info.Sender,true);
        }
        #endregion
        
        
        
    }
}