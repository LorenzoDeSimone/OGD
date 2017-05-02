using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;
using System;
using System.Collections;

public class Collectable : NetworkBehaviour
{
    public int pointValue = 1;
    public int pointScaler = 1;

    private void OnTriggerEnter2D(Collider2D coll)
    {
        MobilePlayerController player = coll.gameObject.GetComponent<MobilePlayerController>();

        if (player && coll.Equals(player.GetCharacterCircleCollider2D()))
        {
            Debug.Log("Collision with Player");

            try
            {
                AddPointsToPlayer(coll.gameObject.GetComponent<PlayerDataHolder>());
            }
            catch (Exception e)
            {
                Debug.LogWarning("Missing Player Data Holder or something really bad!!!\nMessage: "+e.Message);
            }

            SetStateOverNetwork(false);
        }
    }

    private void AddPointsToPlayer(PlayerDataHolder playerDataHolder)
    {
        Debug.Log("Point added");
        playerDataHolder.AddPoints(pointValue * pointScaler);
    }
    
    public void SetStateOverNetwork(bool state)
    {
        if (isServer)
        {
            Debug.Log("On Server");
            RpcChangeState(state);
        }
        else
        {
            Debug.Log("On Client");
            CmdDeactivateThis(state);
        }

    }

    [ClientRpc]
    private void RpcChangeState(bool state)
    {
        gameObject.SetActive(state);
    }

    [Command]
    private void CmdDeactivateThis(bool state)
    {
        gameObject.SetActive(state);
    }
}