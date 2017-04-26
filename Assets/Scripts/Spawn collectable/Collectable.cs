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
            NetworkManager.singleton.GetComponent<PointManager>().addPoint(
                coll.gameObject.transform, 1);
            gameObject.SetActive(false);
        }
    }
}
