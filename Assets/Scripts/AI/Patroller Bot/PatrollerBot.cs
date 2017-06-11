using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollerBot : MonoBehaviour
{
    private Movable movement;
    Movable.CharacterInput input;
    
    void Start()
    {
        movement = GetComponent<Movable>();
        input = new Movable.CharacterInput();
        input.jump = false;
        input.counterClockwise = false;
        input.clockwise = true;
    }

    void Update()
    {
        movement.Move(input);
    }
}
