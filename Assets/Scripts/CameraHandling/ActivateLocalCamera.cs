using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CameraHandling
{
    public class ActivateLocalCamera : NetworkBehaviour
    {
        public GameObject localCamera;

        public override void OnStartLocalPlayer()
        {
            Debug.Log("LOCAL:" + isLocalPlayer);
            GameObject go = Instantiate(localCamera);
            go.GetComponent<FollowPlayer>().playerTransform = transform;
            go.SetActive(true);
        }
    }

}
