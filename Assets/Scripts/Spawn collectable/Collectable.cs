using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;
using System;

namespace Assets.Scripts.Spawn_collectable
{
    public class Collectable : NetworkBehaviour
    {
        [SyncVar( hook = "UpdateNetworkState")]
        bool networkActiveState;

        public int pointValue = 1;
        public int pointScaler = 1;

        SpriteRenderer sprite;
        Collider2D coll;

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            coll = GetComponent<Collider2D>();
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
                Debug.LogWarning("Set false network state");
                networkActiveState = false;
            }
        }

        private void UpdateNetworkState(bool b)
        {
            if (isServer)
                RpcChangeNetworkState(b);
            else
                networkActiveState = b;
        }

        public bool GetNetworkActiveState()
        {
            return networkActiveState;
        }

        [ClientRpc]
        public void RpcChangeNetworkState(bool b)
        {
            networkActiveState = b;
            sprite.enabled = b;
            coll.enabled = b;
        }

        private void AddPointsToPlayer(PlayerDataHolder playerDataHolder)
        {
            playerDataHolder.AddPoints(pointValue * pointScaler);
        }
    }
}