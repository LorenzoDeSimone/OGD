using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;

public class Collectable : NetworkBehaviour
{
    /*
    [SyncVar]
    private bool activeState = true;
    */

    private void OnTriggerEnter2D(Collider2D coll)
    {
        MobilePlayerController player = coll.gameObject.GetComponent<MobilePlayerController>();

        if (player && coll.Equals(player.GetCharacterCircleCollider2D()))
        {
            Debug.Log("Collision with Player");

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