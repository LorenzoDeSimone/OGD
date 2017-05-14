using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Assets.Scripts.Player
{
    class PlayerDataHolder : NetworkBehaviour
    {
        public delegate void OnPointSyncEvent(int playerNetID, int playerPoints);
        public static event OnPointSyncEvent PointSyncEvent;

        [SyncVar (hook = "SendPointSyncEvent")]
        int syncPoints = 0;
        
        [SyncVar]
        public int playerId = 0;
        public bool paintsThePlayer = true;

        private void Start()
        {
            InitPlayer();
        }

        [Command]
        public void CmdAddPoints(int pointsToAdd)
        {
            syncPoints += pointsToAdd;
        }

        //argument needed from sync var PRE-hook... -1 for bar init
        private void SendPointSyncEvent(int newValue)
        {
            PointSyncEvent.Invoke(GetPlayerNetworkId(), newValue);
        }

        private void InitPlayer()
        {
            TryToPaintPlayer();
            //Send event with -1 for bar init
            SendPointSyncEvent(-1);
        }

        private void TryToPaintPlayer()
        {
            if (paintsThePlayer)
            {
                try
                {
                    GetComponent<SpriteRenderer>().color = PlayerColor.GetColor(GetPlayerNetworkId());
                }
                catch
                { /*is this so bad*/}
            }
        }

        public int GetPlayerNetworkId()
        {
            return playerId;
        }
    }
}
