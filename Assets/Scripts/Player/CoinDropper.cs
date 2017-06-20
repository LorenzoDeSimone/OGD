using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Assets.Scripts.Spawn_collectable;
using UnityEngine;
using UnityEngine.Networking;

public class CoinDropper : NetworkBehaviour
{
    /*public 
        PlayerNumberSelector/ma
        num coin da droppare
        max coin da droppare
        coin da droppare (num/max) + (num % max);
    
    CoinDropper
        (coin da droppare / max) +1 -> valore
        quanti coin da droppare  coin da droppare / valore
    */
    public int maxCoinsToDrop = 5;

    public struct CoinToDropInfos
    {
        public Vector3 airPoint;
        public Vector3 groundPoint;
        public int value;
    }

    public List<CoinToDropInfos> coinsToSpawn;

    void Start()
    {
        coinsToSpawn = new List<CoinToDropInfos>();
        StartCoroutine(DropCoinsContinously());
    }

    public void DropCoins(int coinsToDrop)
    {
        GameObject go;
        Vector3 movementVersor;
        Vector3 playerExtents = PlayerDataHolder.GetLocalPlayer().GetComponent<Collider2D>().bounds.extents;
        RaycastHit2D myGround = GetComponentInChildren<Radar>().GetMyGround();
        int valueOfCoinsToDrop = ((coinsToDrop / maxCoinsToDrop)) +1;
        coinsToDrop = coinsToDrop / valueOfCoinsToDrop;

        for (int i = 0; i < coinsToDrop; i++)
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

            //Elevates the ground point to the center of the dropped coin transform
            groundPoint = groundPointHit2D.point + groundPointHit2D.normal * 0.5f;//go.GetComponent<Collider2D>().bounds.extents.y;

            //go = Instantiate((GameObject)Resources.Load("Prefabs/Collectables/DroppedCoin"), transform.position, Quaternion.identity);

            CoinToDropInfos newCoinToSpawn;
            newCoinToSpawn.airPoint = airPoint;
            newCoinToSpawn.groundPoint = groundPoint;
            newCoinToSpawn.value = valueOfCoinsToDrop;
            coinsToSpawn.Add(newCoinToSpawn);

            //Debug.DrawLine(transform.position, airPoint, Color.cyan);
            //Debug.DrawLine(airPoint, groundPoint, Color.green);
        }
    }

    private IEnumerator DropCoinsContinously()
    {    
        while(true)
        {
            if(coinsToSpawn.Count > 0)
            {
                CoinToDropInfos coinInfos = coinsToSpawn[0];
                coinsToSpawn.Remove(coinInfos);
                GameObject go = Instantiate((GameObject)Resources.Load("Prefabs/Collectables/DroppedCoin"), transform.position, Quaternion.identity);
                DroppedCoin droppedCoin = go.GetComponent<DroppedCoin>();
                droppedCoin.pointValue = coinInfos.value;
                droppedCoin.SetCurvePoints(transform.position, coinInfos.airPoint, coinInfos.groundPoint);
                NetworkServer.Spawn(go);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
