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
                Movable.CharacterInput missileDirection;
                missileDirection.clockwise = missileDirection.counterClockwise = missileDirection.jump = false;

                if (currShootPosition.Equals(leftShootPosition))
                    missileDirection.counterClockwise = true;
                else
                    missileDirection.clockwise = true;

                CmdShoot(currShootPosition.transform.position, missileDirection);
            }
        }

        [Command]
        public void CmdShoot(Vector3 clientCurrShootPosition, Movable.CharacterInput missileDirection)
        {
                GameObject playerMissile = (GameObject) Instantiate(Resources.Load("Prefabs/NPCs/PlayerMissile"));
                playerMissile.transform.position = clientCurrShootPosition;
                playerMissile.transform.right = (clientCurrShootPosition - transform.position).normalized;
                playerMissile.GetComponent<PlayerMissile>().SetDirection(missileDirection);
                playerMissile.gameObject.SetActive(true);
                NetworkServer.Spawn(playerMissile);
        }

        public bool CanShoot()
        {
            return true;//Placeholder before missile count implementation
        }

}

}