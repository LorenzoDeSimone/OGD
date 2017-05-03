using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    abstract class PlayMenu : NetworkBehaviour
    {
        protected NetworkLobbyController lobbyController;

        void OnEnable()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            lobbyController.currentPlayMenu = this;
            InitMenu();
        }

        internal void StopMatchSearch()
        {
            StopAllCoroutines();
        }

        protected abstract void InitMenu();
    }
}
