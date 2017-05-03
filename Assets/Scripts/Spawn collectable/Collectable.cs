using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Player;
using System;

namespace Assets.Scripts.Spawn_collectable
{
    public class Collectable : NetworkBehaviour
    {
        [SyncVar( hook = "ChangeNetworkState")]
        bool networkActiveState;

        public int pointValue = 1;
        public int pointScaler = 1;

        SpriteRenderer sprite;
        Collider2D coll;

        private void Start()
        {
            sprite = GetComponent<SpriteRenderer>();
            coll = GetComponent<Collider2D>();
            UpdateNetworkState(false);
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

                UpdateNetworkState(false);
            }
        }

        public void UpdateNetworkState(bool b)
        {
            networkActiveState = b;
            ChangeNetworkState(b);
        }

        public bool GetNetworkActiveState()
        {
            return networkActiveState;
        }

        private void ChangeNetworkState(bool state)
        {
            sprite.enabled = state;
            coll.enabled = state;
        }

        private void AddPointsToPlayer(PlayerDataHolder playerDataHolder)
        {
            Debug.Log("Point added");
            playerDataHolder.AddPoints(pointValue * pointScaler);
        }
    }
}