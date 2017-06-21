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
        protected bool networkActiveState = true;

        public int pointValue = 1;
        public int pointScaler = 1;

        protected SpriteRenderer mySprite;
        protected Collider2D myCollider;
        protected AudioSource audioSource;

        private void Start()
        {
            Init();
        }

        protected void Init()
        {
            mySprite = GetComponent<SpriteRenderer>();
            myCollider = GetComponent<Collider2D>();
            audioSource = GetComponent<AudioSource>();
        }

        public void OnTriggerStay2D(Collider2D coll)
        {
            PlayerDataHolder player = coll.gameObject.GetComponent<PlayerDataHolder>();
            ChaserBot bot = coll.gameObject.GetComponent<ChaserBot>();

            if (player)
                CmdUpdateServerState(false, coll.gameObject.GetComponent<PlayerDataHolder>().playerId);
            if (bot)
                bot.OnCollectableHit(gameObject);
        }

        private void UpdateNetworkState(bool b)
        {
            networkActiveState = b;
            if (isServer)
                RpcChangeNetworkState(b, transform.position); 
        }

        [Command]
        public void CmdUpdateServerState(bool b, int id)
        {
            RealUpdate(b, id);
        }

        protected virtual void RealUpdate(bool b, int id)
        {
            networkActiveState = b;
            mySprite.enabled = b;
            myCollider.enabled = b;

            if (id!=-1)//Special Case: collected by NPC
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
                {
                    PlayerDataHolder pDH = go.GetComponent<PlayerDataHolder>();
                    if (pDH.playerId == id)
                    {
                        pDH.CmdAddPoints(pointValue * pointScaler);
                        break;
                    }
                } 
            }
            PlaySound();
        }

        public bool GetNetworkActiveState()
        {
            return networkActiveState;
        }

        [ClientRpc]
        public void RpcChangeNetworkState(bool b, Vector3 v)
        {
            if (mySprite)
            {
                mySprite.enabled = b;
                myCollider.enabled = b;
                transform.position = v;
            }
        }
        
        protected void PlaySound()
        {
            audioSource.Play();
        }

        private void AddPointsToPlayer(PlayerDataHolder playerDataHolder)
        {
            playerDataHolder.CmdAddPoints(pointValue * pointScaler);
        }
    }
}