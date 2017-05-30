using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Assets.Scripts.Player
{
    class PlayerDataHolder : NetworkBehaviour
    {
        public delegate void OnPointSyncEvent(int playerNetID, int playerPoints);
        public static event OnPointSyncEvent PointSyncEvent;

        private static GameObject localPlayer;

        [SyncVar (hook = "SendPointSyncEvent")]
        int playerPoints = 0;
        
        [SyncVar]
        public int playerId = 0;
        public bool paintsThePlayer = true;

        [Header("Sprites and Animator")]
        public Sprite[] playerSprites;
        //public RuntimeAnimatorController[] animatorControllers;
        
        private void Start()
        {
            if (isLocalPlayer)
                SetLocalPlayer(gameObject);

            InitPlayer();
        }

        private void OnGUI()
        {
            GUI.Box(new Rect(10, 10*playerId, 100, 90), string.Format("{0} {1}", playerPoints, playerId));
        }

        public void AddPoints(int pointsToAdd)
        {
            playerPoints += pointsToAdd;
        }

        //argument needed from sync var PRE-hook... -1 for bar init
        private void SendPointSyncEvent(int newValue)
        {
            if(newValue > 0)
                playerPoints = newValue;
            PointSyncEvent.Invoke(GetPlayerNetworkId(), newValue);
        }

        private void InitPlayer()
        {
            TryToPaintPlayer();
            AddSprite();
            //Send event with -1 for bar init
            SendPointSyncEvent(-1);
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

        public static GameObject GetLocalPLayer()
        {
            return localPlayer;
        }
    }
}
