﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;


public class BotHitter : MonoBehaviour
{
    private PatrollerBot myBot;

    // Use this for initialization
    void Start()
    {
        myBot = GetComponentInParent<PatrollerBot>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerDataHolder player = collider.gameObject.GetComponent<PlayerDataHolder>();
        PlayerMissile playerMissile = collider.gameObject.GetComponent<PlayerMissile>();

        if (myBot.isServer)//Only server can check bot collisions
        {
            if (player)
            {
                player.OnHit();
                myBot.SetPlayerHit(true);
                StartCoroutine(StandStill(myBot.standStillTime));
            }
            else if (playerMissile)
            {
                myBot.DestroyBot();
            }
        }
    }

    IEnumerator<WaitForSeconds> StandStill(float standStillTime)
    {
        yield return new WaitForSeconds(standStillTime);
        myBot.SetPlayerHit(false);
    }
}