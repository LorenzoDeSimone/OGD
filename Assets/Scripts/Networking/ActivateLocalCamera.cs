using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class ActivateLocalCamera : NetworkBehaviour
    {
        public Camera localCamera;

        void Start()
        {
            if (isLocalPlayer)
            {
                localCamera.gameObject.SetActive(true);
            }
        }
    }

}
