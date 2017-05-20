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
            myTargetMarker = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Target Marker"));
            myRadar = GetComponentInChildren<Radar>();
            playerData = GetComponentInParent<PlayerDataHolder>();
        }

        void Update()
        {
            nearestTarget = myRadar.GetNearestTarget();
            if(isLocalPlayer)
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
                GameObject missile = (GameObject)Instantiate(Resources.Load("Prefabs/NPCs/Missile"));
                missile.transform.position = transform.position;
                missile.GetComponent<Missile>().target = nearestTarget;
                missile.GetComponent<Missile>().SetPlayerWhoShot(playerData.playerId);
                missile.gameObject.SetActive(true);
                NetworkServer.Spawn(missile); 
            }
        }

        public bool CanShoot()
        {
            return nearestTarget!=null;//Placeholder before missile count implementation
        }

}

}