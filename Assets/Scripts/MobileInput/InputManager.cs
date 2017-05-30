using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InputManager : NetworkBehaviour {

    private static bool counterclockwisePressed=false, clockwisePressed=false;
    private InputController localPlayerInputController;
    private ShootingController localPlayerShootingController;

    // Use this for initialization
    void Start ()
    {
        //Searches for local player Game Object and stores it
        List < UnityEngine.Networking.PlayerController > players = NetworkManager.singleton.client.connection.playerControllers;
        localPlayerInputController = players[0].gameObject.GetComponent<InputController>();
        localPlayerShootingController = players[0].gameObject.GetComponent<ShootingController>();
    }

    //Handles continous movement
    void Update()
    {
        Movable.CharacterInput input;
        input.counterClockwise = input.clockwise = input.jump = false;

        if (counterclockwisePressed || Input.GetKey(KeyCode.LeftArrow))
        {
            input.counterClockwise = true;
            localPlayerInputController.RequestMovement(input);
        }
        else if (clockwisePressed   || Input.GetKey(KeyCode.RightArrow))
        {
            input.clockwise = true;
            localPlayerInputController.RequestMovement(input);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            input.jump = true;
            localPlayerInputController.RequestMovement(input);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            //Debug.LogError("Shoot");
            localPlayerShootingController.Shoot();
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
        localPlayerShootingController.Shoot();
    }

    public void SetJumpButton()
    {
        Movable.CharacterInput input;
        input.counterClockwise = input.clockwise = false;
        input.jump = true;
        localPlayerInputController.RequestMovement(input);
    }
}
