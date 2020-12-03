using System;
using Draw;
using Photon.Realtime;
using UnityEngine;

namespace Networks
{
    public class NetworkDraw: MonoBehaviour
    {
        public NetworkRPC networkRPC;
        


        private void Awake()
        {
            networkRPC.Clear += OnClear;
            networkRPC.DrawCircle += OnDrawCircle;
            networkRPC.DrawDrag += OnDrawDrag;
            networkRPC.InputDownStatus += OnInputDownStatus;
        }

     

        private void Start()
        {
            DrawManager.Instance.Clear += OnClearMaster;
            DrawManager.Instance.DrawCircle += OnDrawCircleMaster;
            DrawManager.Instance.DrawDrag += OnDrawDragMaster;
            DrawManager.Instance.InputDownStatus += OnInputDownStatusMaster;
        }

        private void OnClearMaster()
        {
            networkRPC.RPC_Clear();
        }

        private void OnDrawCircleMaster(Vector2 obj)
        {
            networkRPC.RPC_DrawCircle(obj);
        }

        private void OnDrawDragMaster(Vector2 arg1, Vector2 arg2)
        {
            networkRPC.RPC_DrawDrag(arg1,arg2);
        }        
        
        private void OnInputDownStatusMaster()
        {
            networkRPC.RPC_InputDownStatus();
        }

        private void OnClear()
        {
            DrawManager.Instance.ClearTexture(true);
        }

        private void OnDrawCircle(Vector2 obj, Player player, bool isRPC)
        {
            DrawManager.Instance.InputDown(obj,player,isRPC);
        }

        private void OnDrawDrag(Vector2 arg1, Vector2 arg2,Player player,bool isRPC)
        {
            DrawManager.Instance.InputDrag(arg1,arg2, player,isRPC );
        }
        
        private void OnInputDownStatus(Player player,bool isRPC)
        {
            DrawManager.Instance.SetInputDownStatus(player,isRPC);
        }
    }
}