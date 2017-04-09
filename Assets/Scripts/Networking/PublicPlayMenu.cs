using UnityEngine;

namespace Assets.Scripts.Networking
{
    class PublicPlayMenu : MonoBehaviour
    {
        public string lobbyControllerTag = "NetworkLobbyController";

        void OnEnable()
        {
            GetLobbyController().JoinPublicMatch();
        }

        protected NetworkLobbyController GetLobbyController()
        {
            GameObject go = GameObject.FindGameObjectWithTag(lobbyControllerTag);
            return go.GetComponent<NetworkLobbyController>();
        }
    }
}
