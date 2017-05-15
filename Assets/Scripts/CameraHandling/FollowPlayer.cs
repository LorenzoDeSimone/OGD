using UnityEngine;

namespace Assets.Scripts.CameraHandling
{
    class FollowPlayer : MonoBehaviour
    {
        public Transform playerTransform;
        public float zOffset = -10;

        Transform tr;
        Vector3 newPosition;

        void Start()
        {
            tr = GetComponent<Transform>();
            newPosition = new Vector3(0, 0, zOffset);
        }

        void Update()
        {
            if (playerTransform)
            {
                newPosition.x = playerTransform.position.x;
                newPosition.y = playerTransform.position.y;
                tr.position = newPosition; 
            }
        }
    }
}
