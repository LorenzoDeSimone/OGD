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
        public float minDroppedCoinSpeed, maxDroppedCoinSpeed, disabledControlsTimeWindow= 0.5f;

        [SyncVar(hook = "SyncNewPoints")]
        int playerPoints = 0;
        [SyncVar(hook = "SyncMissile")]
        bool playerMissile = false;

        [SyncVar]
        public int playerId = 0;
        public bool paintsThePlayer = true;

        private void Start()
        {
            if (isLocalPlayer)
                SetLocalPlayer(gameObject);

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
            localPlayer.GetComponent<Movable>().hit = true;
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

        public Vector3 GetRandomVersor()
        {
            if (Random.Range(0f, 1f) >= 0.5f)
                return (transform.up + transform.right).normalized;
            else
                return (transform.up - transform.right).normalized;
            //float randomAngle = Random.Range(0f, Mathf.PI * 2);
            //return new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized;
        }

        private void DropCoins(int malus)
        {
            GameObject go;
            Vector2 movementVersor;
            Vector3 playerExtents = localPlayer.GetComponent<Collider2D>().bounds.extents;
            for (int i = 0; i < malus; i++)
            {

                movementVersor = GetRandomVersor();
                //((transform.up    * Random.Range(playerExtents.y, playerExtents.y + 1f) +
                //transform.right   * Random.Range(-playerExtents.x, playerExtents.x)) - transform.position).normalized;
                Debug.DrawRay(transform.position, movementVersor, Color.cyan);

                //(Vector3)(Random.Range(2.0f, 5.0f)*Random.insideUnitCircle());
                go = Instantiate((GameObject)Resources.Load("Prefabs/Collectables/DroppedCoin"), transform.position, Quaternion.identity);
                DroppedCoin droppedCoin = go.GetComponent<DroppedCoin>();
                droppedCoin.SetMovementVersor(movementVersor);
                go.GetComponent<Movable>().speed = Random.Range(minDroppedCoinSpeed, maxDroppedCoinSpeed);
                //Debug.LogError("w");
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
    }
}
