using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class PlayerMissileHitter : MonoBehaviour
{
    public float despawnTime = 10f;
    public GameObject explosion;
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
                GameObject go = Instantiate(explosion, player.transform.position, transform.rotation);
                NetworkServer.Spawn(go);
                myMissile.DestroyMissile();
            }
            else if (otherPlayerMissile)
            {
                GameObject go = Instantiate(explosion, transform.position, transform.rotation);
                NetworkServer.Spawn(go);
                myMissile.DestroyMissile();
            }
        }
    }
}