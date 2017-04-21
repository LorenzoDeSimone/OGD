using UnityEngine;

namespace Assets.Scripts.Networking
{
    abstract class PlayMenu : MonoBehaviour
    {
        public string lobbyControllerTag = "NetworkLobbyController";
        protected NetworkLobbyController lobbyController;

        void OnEnable()
        {
            lobbyController = GetLobbyController();
            InitMenu();
        }

        protected abstract void InitMenu();

        protected NetworkLobbyController GetLobbyController()
        {
            GameObject go = GameObject.FindGameObjectWithTag(lobbyControllerTag);
            return go.GetComponent<NetworkLobbyController>();
        }
    }
}
