using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Assets.Scripts.Spawn_collectable;
using UnityEngine;
using UnityEngine.Networking;

public class CoinDropper : NetworkBehaviour
{
    public void DropCoins(int malus)
    {
        GameObject go;
        Vector3 movementVersor;
        Vector3 playerExtents = PlayerDataHolder.GetLocalPlayer().GetComponent<Collider2D>().bounds.extents;
        RaycastHit2D myGround = GetComponentInChildren<Radar>().GetMyGround();

        for (int i = 0; i < malus; i++)
        {
            Vector3 airPoint, groundRaycastStartPoint, groundPoint;

            if (Random.Range(0f, 1f) >= 0.5f)
            {
                movementVersor = (transform.up + transform.right).normalized;
                airPoint = transform.position + movementVersor * Random.Range(playerExtents.y * 1.5f, playerExtents.y * 3);
                groundRaycastStartPoint = transform.position + transform.right * Random.Range(playerExtents.x * 10f, playerExtents.x * 20f);
            }
            else
            {
                movementVersor = (transform.up - transform.right).normalized;
                airPoint = transform.position + movementVersor * Random.Range(playerExtents.y * 1.5f, playerExtents.y * 3);
                groundRaycastStartPoint = transform.position - transform.right * Random.Range(playerExtents.x * 10f, playerExtents.x * 20f);
            }

            RaycastHit2D groundPointHit2D = Physics2D.Raycast(groundRaycastStartPoint,
                                                              myGround.collider.transform.position - groundRaycastStartPoint,
                                                              Mathf.Infinity,
                                                              LayerMask.GetMask("Walkable"));

            go = Instantiate((GameObject)Resources.Load("Prefabs/Collectables/DroppedCoin"), transform.position, Quaternion.identity);

            //Elevates the ground point to the center of the dropped coin transform
            groundPoint = groundPointHit2D.point + groundPointHit2D.normal * go.GetComponent<Collider2D>().bounds.extents.y;
            Debug.DrawLine(transform.position, airPoint, Color.cyan);
            Debug.DrawLine(airPoint, groundPoint, Color.green);
            DroppedCoin droppedCoin = go.GetComponent<DroppedCoin>();
            NetworkServer.Spawn(go);
            droppedCoin.SetCurvePoints(transform.position, airPoint, groundPoint);
        }
    }
}
