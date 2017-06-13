using UnityEngine;
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
            GetComponent<Movable>().hit = true;
            StartCoroutine(StopPlayerMovement(disabledControlsTimeWindow));
            SyncNewPoints(playerPoints);
        }
        
        private IEnumerator StopPlayerMovement(float disabledControlsTimeWindow)
        {
            yield return new WaitForSecondsRealtime(disabledControlsTimeWindow);
            localPlayer.GetComponent<Movable>().hit = false;
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
            Vector3 movementVersor;
            Vector3 playerExtents = localPlayer.GetComponent<Collider2D>().bounds.extents;
            RaycastHit2D myGround= GetComponentInChildren<Radar>().GetMyGround();

            for (int i = 0; i < malus; i++)
            {
                Vector3 airPoint, groundRaycastStartPoint, groundPoint;

                if (Random.Range(0f, 1f) >= 0.5f)
                {
                    movementVersor = (transform.up + transform.right).normalized;
                    airPoint = transform.position + movementVersor * Random.Range(playerExtents.y * 1.5f, playerExtents.y * 3);
                    groundRaycastStartPoint = transform.position + transform.right * Random.Range(playerExtents.x * 10f, playerExtents.x * 20f);
                }
                else
                {
                    movementVersor = (transform.up - transform.right).normalized;
                    airPoint = transform.position + movementVersor * Random.Range(playerExtents.y * 1.5f, playerExtents.y * 3);
                    groundRaycastStartPoint = transform.position - transform.right * Random.Range(playerExtents.x * 10f, playerExtents.x * 20f);
                }

                RaycastHit2D groundPointHit2D = Physics2D.Raycast(groundRaycastStartPoint,
                                                                  myGround.collider.transform.position - groundRaycastStartPoint,
                                                                  Mathf.Infinity,
                                                                  LayerMask.GetMask("Walkable"));

                

                groundPoint = groundPointHit2D.point;
                //((transform.up    * Random.Range(playerExtents.y, playerExtents.y + 1f) +
                //transform.right   * Random.Range(-playerExtents.x, playerExtents.x)) - transform.position).normalized;
                //Debug.DrawRay(transform.position, movementVersor, Color.cyan);
                Debug.DrawLine(transform.position, airPoint, Color.cyan);
                Debug.DrawLine(airPoint, groundPoint, Color.green);

                go = Instantiate((GameObject)Resources.Load("Prefabs/Collectables/DroppedCoin"), transform.position, Quaternion.identity);
                DroppedCoin droppedCoin = go.GetComponent<DroppedCoin>();
                NetworkServer.Spawn(go);
                droppedCoin.SetCurvePoints(transform.position, airPoint, groundPoint);
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
