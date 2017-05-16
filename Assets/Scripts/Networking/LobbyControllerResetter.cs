using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    class LobbyControllerResetter : MonoBehaviour
    {
        bool first = true;

        private void OnEnable()
        {
            if (!first)
            {
                ((NetworkLobbyController)NetworkManager.singleton).ResetNetworkState();
                first = false;
            }
        }
    }
}
