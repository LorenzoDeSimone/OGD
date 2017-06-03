using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Assets.Scripts.Player
{
    class PlayerDataHolder : NetworkBehaviour
    {
        private static GameObject localPlayer;

        [SyncVar (hook = "SyncNewPoints")]
        int playerPoints = 0;
        
        [SyncVar]
        public int playerId = 0;
        public bool paintsThePlayer = true;

        [Header("Sprites and Animators")]
        public Sprite[] playerSprites;
        public RuntimeAnimatorController[] animatorControllers;
        
        private void Start()
        {
            if (isLocalPlayer)
                SetLocalPlayer(gameObject);

            InitPlayer();
        }

        /*
        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10*playerId, 100, 90), string.Format("{0} {1}", playerPoints, playerId));
        }
        */

        public void AddPoints(int pointsToAdd)
        {
            playerPoints += pointsToAdd;
        }

        public void OnHit(int value)
        {
            CmdDecresePoints(value);
            SyncNewPoints(playerPoints);
        }

        [Command]
        private void CmdDecresePoints(int value)
        {
            System.Random rand = new System.Random();
            int matchSize = (int)NetworkManager.singleton.matchSize;
            playerPoints -= rand.Next(2,5) + matchSize - PointManager.instance.GetPlayerRankPosition(GetPlayerNetworkId(),matchSize);
        }

        //argument needed from sync var PRE-hook... -1 for bar init
        private void SyncNewPoints(int newValue)
        {
            if(newValue > 0)
                playerPoints = newValue;
            PointManager.instance.UpdateBar(GetPlayerNetworkId(), newValue);
        }

        private void InitPlayer()
        {
            TryToPaintPlayer();
            AddSprite();
            //Send event with -1 for bar init
            SyncNewPoints(-1);
        }

        private void AddSprite()
        {
            Sprite newSprite = playerSprites[playerId % playerSprites.Length];
            gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
            /*
            RuntimeAnimatorController newController = animatorControllers[playerId % animatorControllers.Length];
            gameObject.GetComponent<Animator>().runtimeAnimatorController = newController;
            */
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
    }
}
