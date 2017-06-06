using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissileHitter : MonoBehaviour
{
    public float despawnTime = 10f;
    PlayerMissile myMissile;
    public float minTimeForExplosion = 0.1f;
    bool canExplode = false;

    private void Start()
    {
        myMissile = GetComponentInParent<PlayerMissile>();
        StartCoroutine(CanExplodeCooldown(minTimeForExplosion));
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerDataHolder player = collider.gameObject.GetComponent<PlayerDataHolder>();
        PlayerMissile otherPlayerMissile = collider.gameObject.GetComponent<PlayerMissile>();

        if (myMissile.isServer && canExplode)//Only server can check missiles collisions
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

    IEnumerator<WaitForSeconds> CanExplodeCooldown(float minTimeForExplosion)
    {
        yield return new WaitForSeconds(minTimeForExplosion);
        canExplode = true;
    }
}