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
            nearestTarget = myRadar.GetNearestTarget();
            if(isLocalPlayer)
                MarkTarget(nearestTarget);
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

        [Command]
        public void CmdShoot()
        {
            if (CanShoot())
            {
                GameObject objMissile = (GameObject) Resources.Load("Prefabs/NPCs/Missile");
                objMissile.transform.position = currShootPosition.transform.position;
                objMissile.GetComponent<Missile>().target = nearestTarget;
                objMissile.gameObject.SetActive(true);
                GameObject missile = Instantiate(objMissile);
                NetworkServer.Spawn(missile); 
            }
        }

        public bool CanShoot()
        {
            return nearestTarget!=null;//Placeholder before missile count implementation
        }

}

}