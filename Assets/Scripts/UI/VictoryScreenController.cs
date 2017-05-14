using Assets.Scripts.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;

namespace Assets.Scripts.UI
{
    class VictoryScreenController : NetworkBehaviour
    {
        [Header("Time of victory screen in seconds")]
        public float vicotoryScreenTime = 4;

        private NetworkLobbyController lobbyController;

        void Start()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            //StartCoroutine(EndMatch());
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
            if(isServer)
            {
                lobbyController.StopServer();
                lobbyController.ResetNetworkState();
            }
            else
            {
                lobbyController.StopClient();
            }
        }
    }
}
