using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class InputController : NetworkBehaviour
    {
        private Movable myMovable;
        private ShootingController myShootingController;

        [ClientCallback]
        void Start()
        {
            if (isLocalPlayer)
            {
                myMovable = GetComponent<Movable>();
                myShootingController = GetComponent<ShootingController>();
            }
        }

        public void RequestMovement(Movable.CharacterInput input)
        {
            if (isLocalPlayer)
            {
                myShootingController.UpdateShootStartPosition(input);
                myMovable.Move(input);
            }
        }
    }

}