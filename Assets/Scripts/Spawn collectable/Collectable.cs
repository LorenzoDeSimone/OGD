using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;
using System;

namespace Assets.Scripts.Spawn_collectable
{
    public class Collectable : NetworkBehaviour
    {
        /*
         * Sync var flow:
         * client -Command: Can i update this variable? -> Server if yes updates the sync var -> the var is sync from client to server
         * syncvar - calls the hook on server and client -> only the server then calls an rpc on the clients   
         * the hook  
         */
        [SyncVar( hook = "UpdateNetworkState")]
        bool networkActiveState;

        public int pointValue = 1;
        public int pointScaler = 1;

        SpriteRenderer mySprite;
        Collider2D myCollider;

        private void Start()
        {
            mySprite = GetComponent<SpriteRenderer>();
            myCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            MobilePlayerController player = coll.gameObject.GetComponent<MobilePlayerController>();

            if (player && coll.Equals(player.GetCharacterCircleCollider2D()))
            {
                Debug.Log("Collision with Player");

                try
                {
                    AddPointsToPlayer(coll.gameObject.GetComponent<PlayerDataHolder>());
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Missing Player Data Holder or something really bad!!!\nMessage: " + e.Message);
                }

                /*
                 *  When a client needs to update the syn 
                 */
                CmdUpdateServerState(false);
            }
        }

        private void UpdateNetworkState(bool b)
        {
            Debug.LogWarning("Sync net state hook of " + netId.Value);

            if (isServer)
                RpcChangeNetworkState(b); 
        }

        [Command]
        private void CmdUpdateServerState(bool b)
        {
            networkActiveState = b;
            mySprite.enabled = b;
            myCollider.enabled = b;
        }

        public bool GetNetworkActiveState()
        {
            return networkActiveState;
        }

        [ClientRpc]
        public void RpcChangeNetworkState(bool b)
        {
            mySprite.enabled = b;
            myCollider.enabled = b;
        }

        private void AddPointsToPlayer(PlayerDataHolder playerDataHolder)
        {
            playerDataHolder.CmdAddPoints(pointValue * pointScaler);
        }
    }
}