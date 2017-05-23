using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class ShootingController : NetworkBehaviour
    {
        private GameObject myTargetMarker;
        private GameObject nearestTarget;
        private PlayerDataHolder playerData;
        private Radar myRadar;
        private GameObject currShootPosition, leftShootPosition, rightShootPosition;

        void Start()
        {
            myTargetMarker = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Target Marker"));
            myRadar = GetComponentInChildren<Radar>();
            playerData = GetComponentInParent<PlayerDataHolder>();

            leftShootPosition = transform.Find("Left Shoot Position").gameObject;
            currShootPosition = rightShootPosition = transform.Find("Right Shoot Position").gameObject;
        }

        void Update()
        {
            nearestTarget = myRadar.GetNearestTarget(currShootPosition.transform.position);
            if (isLocalPlayer)
            {
                MarkTarget(nearestTarget);
            }
        }

        public void UpdateShootStartPosition(MobilePlayerController.PlayerInput input)
        {
            if (input.counterClockwise)
                currShootPosition = leftShootPosition;
            else if (input.clockwise)
                currShootPosition = rightShootPosition;
        }

        private void MarkTarget(GameObject target)
        {
            //Moves target marker over target
            if (target != null)
            {
                myTargetMarker.gameObject.SetActive(true);
                myTargetMarker.transform.position = target.transform.position;
            }
            else
                myTargetMarker.gameObject.SetActive(false);
        }

        public void Shoot()
        {
            if(isLocalPlayer && CanShoot())
                CmdShoot(currShootPosition.transform.position, nearestTarget);
        }

        [Command]
        public void CmdShoot(Vector3 clientCurrShootPosition, GameObject clientNearestTarget)
        {
                GameObject missile = (GameObject) Instantiate(Resources.Load("Prefabs/NPCs/Missile"));
                missile.transform.position = clientCurrShootPosition;
                missile.GetComponent<Missile>().SetTargetId(clientNearestTarget.GetComponent<PlayerDataHolder>().GetPlayerNetworkId());
                missile.transform.right = (clientNearestTarget.transform.position - clientCurrShootPosition).normalized;
                missile.gameObject.SetActive(true);
                NetworkServer.Spawn(missile);
        }

        public bool CanShoot()
        {
            return nearestTarget!=null;//Placeholder before missile count implementation
        }

}

}