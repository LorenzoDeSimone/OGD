using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class InputManager : NetworkBehaviour {

    private static bool counterclockwisePressed=false, clockwisePressed=false, rocketOnTap=false, jumpOnTap=false;
    private MobilePlayerController localPlayer; 

    // Use this for initialization
    void Start ()
    {
        List<UnityEngine.Networking.PlayerController> players = NetworkManager.singleton.client.connection.playerControllers;
        localPlayer = players[0].gameObject.GetComponent<MobilePlayerController>();
    }

    //Getters used by the MobilePlayerControllerScript
    public static bool IsCounterclockwiseButtonPressed()
    { return counterclockwisePressed; }

    public static bool IsClockwiseButtonPressed()
    { return clockwisePressed; }

    public static bool IsRocketButtonOnTap()
    { return rocketOnTap; }

    public static bool IsJumpButtonOnTap()
    { return jumpOnTap; }

    //Setters value from buttons
    public void SetCounterclockwiseButton(bool isPressed)
    { counterclockwisePressed = isPressed; }

    public void SetClockwiseButton(bool isPressed)
    { clockwisePressed = isPressed; }

    public void SetRocketButton(bool isOnTap)
    { rocketOnTap = isOnTap; }

    public void SetJumpButton(bool isOnTap)
    { jumpOnTap = isOnTap && !jumpOnTap; }
}
