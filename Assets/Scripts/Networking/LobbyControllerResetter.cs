using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    class LobbyControllerResetter : MonoBehaviour
    {
        void Start()
        {
            ((NetworkLobbyController)NetworkManager.singleton).ResetNetworkState();
        }
    }
}
