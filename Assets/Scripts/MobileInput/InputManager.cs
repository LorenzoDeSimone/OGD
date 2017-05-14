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
        if (counterclockwisePressed)
            localPlayer.Move(localPlayer.transform.position, MobilePlayerController.MOVEMENT_DIRECTIONS.COUNTERCLOCKWISE);
        else if (clockwisePressed)
            localPlayer.Move(localPlayer.transform.position,MobilePlayerController.MOVEMENT_DIRECTIONS.CLOCKWISE);
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
