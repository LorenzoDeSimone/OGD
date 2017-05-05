using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class InputManager : NetworkBehaviour {

    private static bool counterclockwisePressed=false, clockwisePressed=false;
    private MobilePlayerController localPlayer; 

    // Use this for initialization
    void Start ()
    {
        //Searches for local player Game Object and stores it
        List<UnityEngine.Networking.PlayerController> players = NetworkManager.singleton.client.connection.playerControllers;
        localPlayer = players[0].gameObject.GetComponent<MobilePlayerController>();
    }

    //Handles continous movement
    void Update()
    {
        /*if (counterclockwisePressed || Input.GetKey(KeyCode.LeftArrow))
            localPlayer.Move(MobilePlayerController.MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE);
        else if (clockwisePressed || Input.GetKey(KeyCode.RightArrow))
            localPlayer.Move(MobilePlayerController.MOVEMENT_DIRECTIONS.CLOCKWISE);

        if (Input.GetKeyDown(KeyCode.Space))
            localPlayer.Jump();*/
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
        //Debug.Log("Shoot!");
        localPlayer.CmdShoot();
    }

    public void SetJumpButton()
    {
        localPlayer.Jump();
    }
}
