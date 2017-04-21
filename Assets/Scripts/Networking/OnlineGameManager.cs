using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


namespace Assets.Scripts.Networking
{
    class OnlineGameManager : NetworkBehaviour
    {
        public string lobbyControllerTag = "NetworkLobbyController";

        [Header("Time of a match in seconds")]
        public float matchTime = 120;

        protected NetworkLobbyController lobbyController;
        
        void Start()
        {
            lobbyController = GetLobbyController();
            NetworkLobbyController.PlayerDisconnectEvent += HandlePlayerDisconnection;
            StartCoroutine(MatchCountDown());
        }

        private void OnDestroy()
        {
            NetworkLobbyController.PlayerDisconnectEvent -= HandlePlayerDisconnection;
        }

        protected NetworkLobbyController GetLobbyController()
        {
            GameObject go = GameObject.FindGameObjectWithTag(lobbyControllerTag);
            return go.GetComponent<NetworkLobbyController>();
        }

        private IEnumerator MatchCountDown()
        {
            yield return new WaitForSeconds(matchTime);
            StartEndMatchProcedures();
        }

        private void HandlePlayerDisconnection(NetworkPlayer player, int playerCount)
        {
            if(playerCount==0)
            {
                Debug.LogError("Player lonely");
                StopAllCoroutines();
                StartEndMatchProcedures();
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

        private void StartEndMatchProcedures()
        {
            Debug.LogError("Match Ended");
            GameObject go = GameObject.FindGameObjectWithTag(lobbyControllerTag);
            go.GetComponent<NetworkLobbyController>().ResetAndStop();
        }
    }
}
