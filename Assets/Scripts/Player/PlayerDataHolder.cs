using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    class PlayerDataHolder : NetworkBehaviour
    {
        public delegate void OnPointSyncEvent(int playerNetID, int playerPoints);
        public static event OnPointSyncEvent PointSyncEvent;

        [SyncVar (hook = "SendPointSyncEvent")]
        int points = 0;

        [SyncVar(hook = "SyncId")]
        int myRealId;

        public bool paintsThePlayer = true;

        void Start()
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

            if(isLocalPlayer)
            {
                CmdSendNewId();
            }

            //Send event with -1 for bar init
            SendPointSyncEvent(-1);
        }

        public int GetPlayerNetworkId()
        {
            if (isLocalPlayer)
            {
                Debug.LogWarning(playerControllerId);
                return playerControllerId;
            }
            else
            {
                Debug.LogWarning(myRealId);
                return myRealId;
            }
        }

        [Command]
        public void CmdAddPoints(int pointsToAdd)
        {
            points += pointsToAdd;
        }

        [Command]
        private void CmdSendNewId()
        {
            myRealId = playerControllerId;
        }

        [ClientRpc]
        private void RpcSyncPoints(int newValue)
        {
            points = newValue;
        }

        [ClientRpc]
        private void RpcSyncId(int newId)
        {
            Debug.LogWarning("AAAAA!");
            myRealId = newId;
        }

        private void SyncId(int newId)
        {
            if (newId != myRealId)
            {
                if (isServer)
                {
                    RpcSyncId(newId);
                } 
            }
        }
        
        //argument needed from sync var PRE-hook... -1 for bar init
        private void SendPointSyncEvent(int newValue)
        {
            if (newValue != points)
            {
                if (isServer)
                {
                    RpcSyncPoints(newValue);
                }

                PointSyncEvent.Invoke(GetPlayerNetworkId(), newValue); 
            }
        }

    }
}
