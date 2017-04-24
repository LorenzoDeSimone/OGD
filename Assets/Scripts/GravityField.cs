using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

public class GravityField : MonoBehaviour {

    public float mass = 50;
    
    /*
    private void OnTriggerEnter2D(Collider2D collider)
    {
        MobilePlayerController player = collider.GetComponent<MobilePlayerController>();
        if (player != null)//If a player has entered in this gravity field attraction area, it changes his gravity field
            player.SetGravityField(this);
    }*/
}
