using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    abstract class PlayMenu : MonoBehaviour
    {
        protected NetworkLobbyController lobbyController;

        void OnEnable()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            InitMenu();
        }

        protected abstract void InitMenu();
    }
}
