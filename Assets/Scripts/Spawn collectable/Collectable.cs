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
            CallAddPointOnServer(coll);
            gameObject.SetActive(false);
        }
    }

    [Server]
    private static void CallAddPointOnServer(Collision2D coll)
    {
        GameObject.FindGameObjectWithTag("OnlineGameManager").GetComponent<PointManager>().addPoint(
            coll.gameObject.transform, 1);
    }
}
