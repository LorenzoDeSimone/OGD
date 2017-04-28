using UnityEngine;
using UnityEngine.Networking;

public class Collectable : NetworkBehaviour
{
    /*
    [SyncVar]
    private bool activeState = true;
    */

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.CompareTag("Player"))
        {
            if(isServer)
            {
                RpcDeactivateThis();
            }
            else
            {
                CmdDeactivateThis();
            }
        }
    }

    /*
    private void Update()
    {
        gameObject.SetActive(activeState);
    }
    */

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

    /*
    private void OnEnable()
    {
        activeState = true;
    }

    private void OnDisable()
    {
        activeState = false;
    }
    */
}