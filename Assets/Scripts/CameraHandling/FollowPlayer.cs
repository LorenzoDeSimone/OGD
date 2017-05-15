using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.CameraHandling
{
    class FollowPlayer : MonoBehaviour
    {
        public Transform playerTransform;
        public float zOffset = -10;
        Transform tr;
        bool firstSnap = true;
        public float snapThreshold = 0.1f;

        [Range(0.01f, 1.0f)]
        public float cameraLerp = 0.1f;

        void Start()
        {
            tr = GetComponent<Transform>();
        }

        void LateUpdate()
        {
            Vector3 newPosition;
            newPosition.x = playerTransform.position.x;
            newPosition.y = playerTransform.position.y;
            newPosition.z = zOffset;

            if (firstSnap)
            {
                tr.position = newPosition;
                firstSnap = false;
            }
            else
                tr.position = Vector3.Lerp(tr.position, newPosition, cameraLerp);
        }
    }
}
