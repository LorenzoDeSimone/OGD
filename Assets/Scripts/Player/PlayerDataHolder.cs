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

            //Send event for bar init
            SendPointSyncEvent(0);
        }

        public void AddPoints(int pointsToAdd)
        {
            points += pointsToAdd;
            Debug.Log(GetPlayerNetworkId()+": "+points);
        }

        public int GetPlayerNetworkId()
        {
            Debug.Log(netId.Value);

            return (int)netId.Value;
        }

        //argument needed from sync var hook... is it of any use?
        private void SendPointSyncEvent( int newValue )
        {
            PointSyncEvent.Invoke(GetPlayerNetworkId(), points);
        }
    }
}
