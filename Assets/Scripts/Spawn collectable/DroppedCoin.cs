using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoin : Collectable
    {
        private void OnTriggerEnter2D(Collider2D coll)
        {
            PlayerDataHolder player = coll.gameObject.GetComponent<PlayerDataHolder>();

            if (player)// && coll.Equals(player.GetCharacterCapsuleCollider2D()))
                CmdUpdateServerState(false, coll.gameObject.GetComponent<PlayerDataHolder>().playerId);
        }


        [Command]
        private void CmdUpdateServerState(bool b, int id)
        {
            networkActiveState = b;
            mySprite.enabled = b;
            myCollider.enabled = b;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                PlayerDataHolder pDH = go.GetComponent<PlayerDataHolder>();
                if (pDH.playerId == id)
                {
                    pDH.AddPoints(pointValue * pointScaler);
                    break;
                }
            }
            NetworkServer.Destroy(gameObject);
        }
    }

}