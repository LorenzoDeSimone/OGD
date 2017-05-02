using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;
using System;

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

            if (isServer)
            {
                Debug.Log("On Server");
                RpcDeactivateThis();
            }
            else
            {
                Debug.Log("On Client");
                CmdDeactivateThis();
            }
        }
    }

    private void AddPointsToPlayer(PlayerDataHolder playerDataHolder)
    {
        playerDataHolder.AddPoints(pointValue * pointScaler);
    }

    [ClientRpc]
    private void RpcDeactivateThis()
    {
        gameObject.SetActive(false);
    }

    [Command]
    private void CmdDeactivateThis()
    {
        gameObject.SetActive(false);
    }
}