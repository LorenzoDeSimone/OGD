using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    [RequireComponent(typeof(OnlineGameManager))]
    class PlayerDisconnectionHandler : NetworkBehaviour
    {
        OnlineGameManager onlineGameManager;
        protected NetworkLobbyController lobbyController;

        void Start()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            onlineGameManager = GetComponent<OnlineGameManager>();

            NetworkLobbyController.PlayerDisconnectEvent += HandlePlayerDisconnection;
        }

        private void OnDestroy()
        {
            NetworkLobbyController.PlayerDisconnectEvent -= HandlePlayerDisconnection;
        }

        private void HandlePlayerDisconnection(NetworkConnection conn, int playerCount)
        {
            Debug.LogWarning("Player: " + conn + " dsconected! only " + playerCount + " little indians remains...");
            

            if (playerCount == 1)
            {
                StopAllCoroutines();
                onlineGameManager.EndMatch();
            }
        }
    }
}
