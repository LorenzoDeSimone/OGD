using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    class PlayerDataHolder : NetworkBehaviour
    {
        private static GameObject localPlayer;
        public PlayerDresser dresser;

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
            SyncNewPoints(playerPoints);
        }

        [Command]
        private void CmdDecresePoints()
        {
            int malus = Random.Range(2, 5) + PointManager.instance.GetMatchSize() - PointManager.instance.GetPlayerRankPosition(GetPlayerNetworkId());

            if (playerPoints - malus < 0)
            {
                DropCoins(playerPoints);
                playerPoints = 0;

            }
            else
            {
                DropCoins(malus);
                playerPoints -= malus;
            }
        }

        private void DropCoins(int malus)
        {
            GameObject go;
            Vector2 newPos;

            for (int i = 0; i < malus; i++)
            {
                newPos = transform.position + transform.up* Random.Range(2.0f, 5.0f) + (Vector3)(Random.Range(2.0f, 5.0f)*Random.insideUnitCircle);
                go = Instantiate((GameObject)Resources.Load("Prefabs/Collectables/DroppedCoin"), newPos, Quaternion.identity);
                NetworkServer.Spawn(go);
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
            Debug.LogWarning("1");
            CmdFlipSprite(b);
        }

        [Command]
        private void CmdFlipSprite(bool b)
        {
            Debug.LogWarning("2");
            flip = b;
        }

        private void SyncFlip(bool b)
        {
            Debug.LogWarning("3");
            flip = b;
            mySpriteRenderer.flipX = b;
        }
    }
}
