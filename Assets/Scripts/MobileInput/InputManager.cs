using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InputManager : NetworkBehaviour {

    private static bool counterclockwisePressed, clockwisePressed;
    private InputController localPlayerInputController = null;
    private ShootingController localPlayerShootingController = null;
    private Animator localPlayerAnimator;

    // Use this for initialization
    void Start ()
    {
        counterclockwisePressed = clockwisePressed = false;
    }

    //Handles continous movement
    void Update()
    {
        if (localPlayerInputController == null)
        {
            if (PlayerDataHolder.GetLocalPlayer())
            {
                localPlayerInputController = PlayerDataHolder.GetLocalPlayer().GetComponent<InputController>();
                localPlayerShootingController = PlayerDataHolder.GetLocalPlayer().gameObject.GetComponent<ShootingController>();
                localPlayerAnimator = PlayerDataHolder.GetLocalPlayer().gameObject.GetComponent<Animator>();
            }
        }
        else
        {

            Movable.CharacterInput input;
            input.counterClockwise = input.clockwise = input.jump = false;

            if (counterclockwisePressed || Input.GetKey(KeyCode.LeftArrow))
            {
                input.counterClockwise = true;
                localPlayerAnimator.SetBool("moving", true);
                localPlayerInputController.RequestMovement(input);
            }
            else if (clockwisePressed || Input.GetKey(KeyCode.RightArrow))
            {
                input.clockwise = true;
                localPlayerAnimator.SetBool("moving", true);
                localPlayerInputController.RequestMovement(input);
            }
            else
            {
                localPlayerAnimator.SetBool("moving", false);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                input.jump = true;
                localPlayerAnimator.SetTrigger("jump");
                localPlayerInputController.RequestMovement(input);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                //Debug.LogError("Shoot");
                localPlayerShootingController.Shoot();
            }
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
        localPlayerAnimator.SetTrigger("jump");
        localPlayerInputController.RequestMovement(input);
    }
}
