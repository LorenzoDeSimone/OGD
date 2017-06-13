using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissileHitter : MonoBehaviour
{
    public float despawnTime = 10f;
    PlayerMissile myMissile;
    Movable myMovable;

    private void Start()
    {
        myMissile = GetComponentInParent<PlayerMissile>();
        myMovable = GetComponent<Movable>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerDataHolder player = collider.gameObject.GetComponent<PlayerDataHolder>();
        PlayerMissile otherPlayerMissile = collider.gameObject.GetComponent<PlayerMissile>();
        if (myMissile.isServer)//Only server can check missiles collisions
        {
            if (player)
            {
                player.OnHit();
                //Debug.LogError("Player Hit! " + target.gameObject.name);
                myMissile.DestroyMissile();
            }
            else if (otherPlayerMissile)
            {
                myMissile.DestroyMissile();
            }
        }
    }
}