using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class ShootingController : NetworkBehaviour
    {
        private PlayerDataHolder playerData;
        private Radar myRadar;
        private GameObject currShootPosition, leftShootPosition, rightShootPosition;
        private Movable myMovable;

        void Start()
        {
            myRadar = GetComponentInChildren<Radar>();
            myMovable = GetComponent<Movable>();

            playerData = GetComponentInParent<PlayerDataHolder>();

            leftShootPosition = transform.Find("Left Shoot Position").gameObject;
            currShootPosition = rightShootPosition = transform.Find("Right Shoot Position").gameObject;
        }

        void Update()
        {

        }

        public void UpdateShootStartPosition(Movable.CharacterInput input)
        {
            if (myMovable.CanMove())
            {
                if (input.counterClockwise)
                    currShootPosition = leftShootPosition;
                else if (input.clockwise)
                    currShootPosition = rightShootPosition;
            }
        }

        public void Shoot()
        {
            if (isLocalPlayer && CanShoot())
            {
                Vector2 missileDirection;
                int missileStatus = PlayerMissile.inAir;

                if (leftShootPosition.Equals(currShootPosition))
                {
                    missileStatus = PlayerMissile.counterclockwise;
                    missileDirection = (leftShootPosition.transform.position - rightShootPosition.transform.position).normalized;
                }
                else
                {
                    missileStatus = PlayerMissile.clockwise;
                    missileDirection = (rightShootPosition.transform.position - leftShootPosition.transform.position).normalized;
                }


                CmdShoot(currShootPosition.transform.position, missileDirection, missileStatus);

            }
        }

        [Command]
        public void CmdShoot(Vector2 shootPosition, Vector2 missileDirection, int missileStatus)
        {
            GameObject playerMissile = (GameObject) Instantiate(Resources.Load("Prefabs/NPCs/PlayerMissile"));
            playerMissile.transform.position = shootPosition;
            playerMissile.transform.right = missileDirection;
            playerMissile.GetComponent<PlayerMissile>().SetStatus(missileStatus);
            playerMissile.gameObject.SetActive(true);
            NetworkServer.Spawn(playerMissile);
        }

        public bool CanShoot()
        {
            return true;//Placeholder before missile count implementation
        }


}

}