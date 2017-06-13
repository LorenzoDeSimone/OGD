﻿using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Spawn_collectable;
using System.Collections;

namespace Assets.Scripts.Player
{
    class PlayerDataHolder : NetworkBehaviour
    {
        private static GameObject localPlayer;
        public PlayerDresser dresser;
        public float disabledControlsTimeWindow= 0.5f;

        [SyncVar(hook = "SyncNewPoints")]
        int playerPoints = 0;
        [SyncVar(hook = "SyncMissile")]
        bool playerMissile = false;
        [SyncVar(hook = "SyncFlip")]
        bool flip;

        [SyncVar]
        public int playerId = 0;
        public bool paintsThePlayer = true;

        SpriteRenderer mySpriteRenderer;

        private void Start()
        {
            if (isLocalPlayer)
                SetLocalPlayer(gameObject);

            mySpriteRenderer = GetComponent<SpriteRenderer>();

            InitPlayer();
        }
        [Command]
        public void CmdAddPoints(int pointsToAdd)
        {
            playerPoints += pointsToAdd;
        }

        [Command]
        public void CmdAddMissile()
        {
            playerMissile = true;
        }

        [Command]
        public void CmdRemoveMissile()
        {
            playerMissile = false;
        }

        public bool PlayerHaveMissile()
        {
            return playerMissile;
        }

        public void SyncMissile(bool b)
        {
            playerMissile = b;
            if (isLocalPlayer)
            {
                if (b)
                    MissileButton.instance.Activate();
                else
                    MissileButton.instance.Deactivate(); 
            }
        }

        public void OnHit()
        {
            CmdDecresePoints();
            StartCoroutine(StopPlayerMovement(disabledControlsTimeWindow));
            SyncNewPoints(playerPoints);
        }
        
        private IEnumerator StopPlayerMovement(float disabledControlsTimeWindow)
        {
            CmdSyncHitBool(true);
            yield return new WaitForSecondsRealtime(disabledControlsTimeWindow);
            CmdSyncHitBool(false);
        }

        [Command]
        private void CmdSyncHitBool(bool b)
        {
            GetComponent<Movable>().hit = b;
        }

        [Command]
        private void CmdDecresePoints()
        {
            int malus = Random.Range(2, 5) + PointManager.instance.GetMatchSize() - PointManager.instance.GetPlayerRankPosition(GetPlayerNetworkId());

            if (playerPoints - malus < 0)
            {
                GetComponent<CoinDropper>().DropCoins(playerPoints);
                playerPoints = 0;

            }
            else
            {
                GetComponent<CoinDropper>().DropCoins(malus);
                playerPoints -= malus;
            }
        }

        //argument needed from sync var PRE-hook... -1 for bar init
        private void SyncNewPoints(int newValue)
        {
            if (newValue > 0)
                playerPoints = newValue;
            if (PointManager.instance != null)
                PointManager.instance.UpdateBar(GetPlayerNetworkId(), newValue);
        }

        private void InitPlayer()
        {
            TryToPaintPlayer();
            dresser.DressPlayer(GetComponent<SpriteRenderer>(), GetPlayerNetworkId());
            GetComponent<Animator>().runtimeAnimatorController = dresser.GetAnimator(GetPlayerNetworkId());
            //Send event with -1 for bar init
            SyncNewPoints(-1);
        }

        private void TryToPaintPlayer()
        {
            if (paintsThePlayer)
            {
                try
                {
                    GetComponent<SpriteRenderer>().color = PlayerColor.GetColor(GetPlayerNetworkId());
                }
                catch
                { /*is this so bad*/}
            }
        }

        public int GetPoints()
        {
            return playerPoints;
        }

        public int GetPlayerNetworkId()
        {
            return playerId;
        }

        private static void SetLocalPlayer(GameObject go)
        {
            localPlayer = go;
        }

        public static GameObject GetLocalPlayer()
        {
            return localPlayer;
        }

        internal void FlipSprite(bool b)
        {
            CmdFlipSprite(b);
        }

        [Command]
        private void CmdFlipSprite(bool b)
        {
            flip = b;
        }

        private void SyncFlip(bool b)
        {
            flip = b;
            mySpriteRenderer.flipX = b;
        }
    }
}
