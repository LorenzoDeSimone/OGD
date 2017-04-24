using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;


namespace Assets.Scripts.Networking
{
    class OnlineGameManager : NetworkBehaviour
    {
        [Header("Time of a match in seconds")]
        public float matchTime = 180;

        [Header("Time of victory screen in seconds")]
        public float vicotoryScreenTime = 4;

        public UnityEvent OnPlayerDisconnects;
        public UnityEvent OnMatchEnded;

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
            OnMatchEnded.Invoke();
            /*
             * This will make all phisics related things to stop 
             */
            Time.timeScale = 0;
            // unscaled time here!! see above
            yield return new WaitForSecondsRealtime(vicotoryScreenTime);
            Time.timeScale = 1;
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
