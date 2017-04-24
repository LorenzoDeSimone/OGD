using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace Assets.Scripts.Networking
{
    class OnlineGameManager : NetworkBehaviour
    {
        [Header("Time of a match in seconds")]
        public float matchTime = 120;

        [Header("Time of victory screen in seconds")]
        public float vicotoryScreenTime = 10;

        protected NetworkLobbyController lobbyController;
        
        void Start()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;

            NetworkLobbyController.PlayerDisconnectEvent += HandlePlayerDisconnection;
            StartCoroutine(StartMatchCountDown());
        }

        private void OnDestroy()
        {
            NetworkLobbyController.PlayerDisconnectEvent -= HandlePlayerDisconnection;
        }
        
        private IEnumerator StartMatchCountDown()
        {
            yield return new WaitForSeconds(matchTime);
            yield return new WaitForSeconds(vicotoryScreenTime);
            StopMatch();
        }

        private void HandlePlayerDisconnection(NetworkPlayer player, int playerCount)
        {
            if(playerCount==0)
            {
                Debug.LogError("Player lonely");
                StopAllCoroutines();
                StopMatch();
            }
            else
            {
                if( player.Equals(lobbyController.GetLocalPlayer()) )
                {
                    Debug.LogError("You disconnected");
                }
                else
                {
                    Debug.LogError("" + player.ToString() + " disconnected");
                }
            }
        }

        private void StopMatch()
        {
            Debug.LogError("Match Ended");
            lobbyController.ResetAndStop();
        }
    }
}
