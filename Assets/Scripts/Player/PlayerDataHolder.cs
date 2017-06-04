using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    class PlayerDataHolder : NetworkBehaviour
    {
        private static GameObject localPlayer;
        public PlayerDresser dresser;

        [SyncVar (hook = "SyncNewPoints")]
        int playerPoints = 0;
        
        [SyncVar]
        public int playerId = 0;
        public bool paintsThePlayer = true;
        
        private void Start()
        {
            if (isLocalPlayer)
                SetLocalPlayer(gameObject);

            InitPlayer();
        }

        public void AddPoints(int pointsToAdd)
        {
            playerPoints += pointsToAdd;
        }

        public void OnHit()
        {
            CmdDecresePoints();
            SyncNewPoints(playerPoints);
        }

        [Command]
        private void CmdDecresePoints()
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
            if(PointManager.instance != null)
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
