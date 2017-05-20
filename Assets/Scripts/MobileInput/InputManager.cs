using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InputManager : NetworkBehaviour {

    private static bool counterclockwisePressed=false, clockwisePressed=false;
    private MobilePlayerController localPlayerMovementController;
    private ShootingController localPlayerShootingController;

    // Use this for initialization
    void Start ()
    {
        //Searches for local player Game Object and stores it
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(go.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayerMovementController = go.GetComponent<MobilePlayerController>();
                localPlayerShootingController = go.GetComponentInChildren<ShootingController>();
                break;
            }
        }
    }

    //Handles continous movement
    void Update()
    {
        MobilePlayerController.PlayerInput input;
        input.counterClockwise = input.clockwise = input.jump = false;
        input.timestamp = Network.time;

        if (counterclockwisePressed || Input.GetKey(KeyCode.LeftArrow))
        {
            input.counterClockwise = true;
            localPlayerMovementController.LocalMoveandStoreInputInBuffer(input);
        }
        else if (clockwisePressed   || Input.GetKey(KeyCode.RightArrow))
        {
            input.clockwise = true;
            localPlayerMovementController.LocalMoveandStoreInputInBuffer(input);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            input.jump = true;
            localPlayerMovementController.LocalMoveandStoreInputInBuffer(input);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            localPlayerShootingController.CmdShoot();
        }
    }

    //Setters value from buttons
    public void SetCounterclockwiseButton(bool isPressed)
    {
        counterclockwisePressed = isPressed;
    }

    public void SetClockwiseButton(bool isPressed)
    {
        clockwisePressed = isPressed;
    }

    public void SetRocketButton()
    {
        localPlayerShootingController.CmdShoot();
    }

    public void SetJumpButton()
    {
        MobilePlayerController.PlayerInput input;
        input.counterClockwise = input.clockwise = false;
        input.jump = true;
        input.timestamp = Network.time;
        localPlayerMovementController.LocalMoveandStoreInputInBuffer(input);
    }
}
