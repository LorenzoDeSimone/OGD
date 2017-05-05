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

            //Send event with -1 for bar init
            SendPointSyncEvent(-1);
        }

        public void AddPoints(int pointsToAdd)
        {
            points += pointsToAdd;
        }

        public int GetPlayerNetworkId()
        {
            return (int)netId.Value;
        }

        //argument needed from sync var PRE-hook... -1 for bar init
        private void SendPointSyncEvent( int newValue )
        {
            points = newValue;
            PointSyncEvent.Invoke(GetPlayerNetworkId(), newValue);
        }
    }
}
