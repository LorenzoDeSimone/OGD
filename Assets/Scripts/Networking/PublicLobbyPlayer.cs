using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    class PublicLobbyPlayer : NetworkLobbyPlayer
    {
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            SendReadyToBeginMessage();
        }
    }
}
