using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Assets.Scripts.Player;

namespace Assets.Scripts.Networking
{
    class OnlineGameManager : NetworkBehaviour
    {
        [Header("Time of a match in seconds")]
        public float matchTime = 180;

        [Header("Time of victory screen in seconds")]
        public float vicotoryScreenTime = 4;

        protected NetworkLobbyController lobbyController;

        public GameObject scoreboardPrefab;
        public GameObject idMasterPrefab;
        
        void Start()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            StartCoroutine(StartMatchCountDown());
        }

        public void EndMatchWrapper(string message)
        {
            Debug.Log(message);
            StartCoroutine(EndMatch());
        }

        private IEnumerator StartMatchCountDown()
        {
            yield return new WaitForSeconds(matchTime);
            EndMatchWrapper("Match ended after count down...");
        }

        private IEnumerator EndMatch()
        {
            lobbyController.PrepareToReset();
            // This will make all physics related things to stop 
            Time.timeScale = 0;
            // unscaled time here!! see above
            yield return new WaitForSecondsRealtime(vicotoryScreenTime);
            Time.timeScale = 1;
            KillNetwork();
        }

        private void KillNetwork()
        {
            Debug.LogWarning("Match Ended");
            lobbyController.ResetNetworkState();
        }
    }
}
