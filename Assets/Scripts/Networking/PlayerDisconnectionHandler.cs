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

        private void HandlePlayerDisconnection(NetworkPlayer player, int playerCount)
        {
            Debug.LogWarning("Player: " + player + " dsconected! only " + playerCount + " little indians remains...");

            if (playerCount == 1)
            {
                Debug.LogError("Player lonely...");
                StopAllCoroutines();
                onlineGameManager.EndMatchWrapper("Match ended because player lonely...");
            }
            else
            {
                if (player.Equals(lobbyController.GetLocalPlayer()))
                {
                    Debug.LogError("You disconnected");
                }
                else
                {
                    Debug.LogError("" + player.ToString() + " disconnected");
                }
            }
        }
    }
}
