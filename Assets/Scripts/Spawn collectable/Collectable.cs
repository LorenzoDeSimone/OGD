using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Collectable : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            CallAddPointOnServer( NetworkClient.allClients[0].connection.connectionId );
            gameObject.SetActive(false);
        }
    }

    [Server]
    private static void CallAddPointOnServer(int connID)
    {
        GameObject.FindGameObjectWithTag("OnlineGameManager").GetComponent<PointManager>().addPoint(
            connID, 1);
    }
}
