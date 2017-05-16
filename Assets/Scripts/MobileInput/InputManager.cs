using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InputManager : NetworkBehaviour {

    private static bool counterclockwisePressed=false, clockwisePressed=false;
    private MobilePlayerController localPlayer; 

    // Use this for initialization
    void Start ()
    {
        //Searches for local player Game Object and stores it
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(go.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                localPlayer = go.GetComponent<MobilePlayerController>();
                break;
            }
        }
    }

    //Handles continous movement
    void Update()
    {
        MobilePlayerController.PlayerInput input;
        input.counterClockwise = input.clockwise = input.jump = input.shoot = false;
        input.timestamp = Network.time;

        if (counterclockwisePressed || Input.GetKey(KeyCode.LeftArrow))
        {
            input.counterClockwise = true;
            localPlayer.LocalMoveandStoreInputInBuffer(input);
        }
        else if (clockwisePressed   || Input.GetKey(KeyCode.RightArrow))
        {
            input.clockwise = true;
            localPlayer.LocalMoveandStoreInputInBuffer(input);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            input.jump = true;
            localPlayer.LocalMoveandStoreInputInBuffer(input);
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
        Debug.Log("Shoot!");
        //localPlayer.CmdShoot();
    }

    public void SetJumpButton()
    {
        MobilePlayerController.PlayerInput input;
        input.counterClockwise = input.clockwise = input.shoot = false;
        input.jump = true;
        input.timestamp = Network.time;
        localPlayer.LocalMoveandStoreInputInBuffer(input);
    }
}
