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

        void Start()
        {
            if (!isLocalPlayer)//Shooting module is needed only for local player
                enabled = false;

            myTargetMarker = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Target Marker"));
            myRadar = GetComponentInChildren<Radar>();
            playerData = GetComponentInParent<PlayerDataHolder>();
        }

        void Update()
        {
            nearestTarget = myRadar.GetNearestTarget();
            MarkTarget(nearestTarget);
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
                GameObject rocket = (GameObject)Instantiate(Resources.Load("Prefabs/NPCs/Rocket"));
                rocket.transform.position = transform.position;
                rocket.GetComponent<Rocket>().target = nearestTarget;
                rocket.GetComponent<Rocket>().SetPlayerWhoShot(playerData.playerId);
                rocket.gameObject.SetActive(true);
                NetworkServer.Spawn(rocket); 
            }
        }

        public bool CanShoot()
        {
            return nearestTarget!=null;//Placeholder before rocket count implementation
        }

}

}