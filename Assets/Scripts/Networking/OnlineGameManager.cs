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

        public UnityEvent OnMatchEnded;

        protected NetworkLobbyController lobbyController;
        
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
            OnMatchEnded.Invoke();
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
