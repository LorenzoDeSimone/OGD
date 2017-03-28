using UnityEngine;

namespace Assets.Scripts.CameraHandling
{
    class FollowPlayer : MonoBehaviour
    {
        public Transform playerTransform;

        Transform tr;

        void Start()
        {
            tr = GetComponent<Transform>();
        }

        void FixedUpdate()
        {
            tr.position = playerTransform.position;
        }
    }
}
