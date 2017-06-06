using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoin : Collectable
    {
        Vector2 direction = Vector2.one;
        bool stop = false;

        private void Start()
        {
            mySprite = GetComponent<SpriteRenderer>();
            myCollider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            PlayerDataHolder player = coll.gameObject.GetComponent<PlayerDataHolder>();

            if (player)// && coll.Equals(player.GetCharacterCapsuleCollider2D()))
                CmdUpdateServerState(false, coll.gameObject.GetComponent<PlayerDataHolder>().playerId);
        }

        private void Update()
        {
            if(!stop)
            {
                transform.position = Vector3.Lerp(transform.position, direction * 1.2f, 0.01f);
            }
        }

        [Command]
        private void CmdUpdateServerState(bool b, int id)
        {
            networkActiveState = b;
            mySprite.enabled = b;
            myCollider.enabled = b;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player"))
            {
                PlayerDataHolder pDH = go.GetComponent<PlayerDataHolder>();
                if (pDH.playerId == id)
                {
                    pDH.AddPoints(pointValue * pointScaler);
                    break;
                }
            }
            NetworkServer.Destroy(gameObject);
        }

        public void Stop()
        {
            stop = true;
        }

        public void SetDirection(Vector2 newDir)
        {
            direction = newDir;
        }
    }

}