using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class ShootingController : NetworkBehaviour
    {
        private GameObject myTargetMarker;
        private HashSet<GameObject> myTargets;//A collection of hittable targets currently in player's trigger
        private GameObject nearestTarget;
        private PlayerDataHolder playerData;

        void Start()
        {
            if (!GetComponentInParent<MobilePlayerController>().isLocalPlayer)//Shooting Script is needed only for local player
                enabled = false;

            myTargets = new HashSet<GameObject>();
            myTargetMarker = (GameObject)Instantiate(Resources.Load("Prefabs/Player/Target Marker"));
            playerData = GetComponentInParent<PlayerDataHolder>();
            nearestTarget = GetNearestTargetAndMarkIt();
        }

        void Update()
        {
            nearestTarget = GetNearestTargetAndMarkIt();
        }

        [Command]
        public void CmdShoot()
        {
            if (nearestTarget == null || !CanShoot())
            {
                //Debug.Log("No targets in my area!");
                return;
            }

            GameObject rocket = (GameObject)Instantiate(Resources.Load("Prefabs/NPCs/Rocket"));
            rocket.transform.position = transform.position;
            rocket.GetComponent<Rocket>().target = nearestTarget;
            rocket.GetComponent<Rocket>().SetPlayerWhoShot(playerData.playerId);
            rocket.gameObject.SetActive(true);

            NetworkServer.Spawn(rocket);
        }

        private GameObject GetNearestTargetAndMarkIt()
        {
            float candidateMinDistance = float.MaxValue;
            GameObject candidateNearestTarget = null;

            foreach (GameObject currTarget in myTargets)
            {
                float currDistance = Vector2.Distance(transform.position, currTarget.transform.position);

                if (currDistance < candidateMinDistance)
                {
                    candidateNearestTarget = currTarget.gameObject;
                    candidateMinDistance = currDistance;
                }
            }

            //Mark nearest target
            if (candidateNearestTarget != null)
            {
                myTargetMarker.gameObject.SetActive(true);
                myTargetMarker.transform.position = candidateNearestTarget.transform.position;
            }
            else
                myTargetMarker.gameObject.SetActive(false);

            return candidateNearestTarget;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            //Target management
            Target newTarget = collider.GetComponent<Target>();
            if (newTarget != null)
                myTargets.Add(newTarget.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            //Target management
            Target target = collider.GetComponent<Target>();
            if (target != null)
                myTargets.Remove(target.gameObject);
        }


        public bool CanShoot()
        {
            return true;//Placeholder before rocket count implementation
        }

}

}