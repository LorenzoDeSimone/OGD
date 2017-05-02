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

        public UnityEvent OnPlayerDisconnects;

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

            OnPlayerDisconnects.Invoke();
            if (playerCount == 1)
            {
                Debug.LogWarning("Player lonely...");
                StopAllCoroutines();
                onlineGameManager.EndMatchWrapper("Match ended because player lonely...");
            }
            else
            {
                Debug.LogError("" + conn.ToString() + " disconnected");
            }
        }
    }
}
