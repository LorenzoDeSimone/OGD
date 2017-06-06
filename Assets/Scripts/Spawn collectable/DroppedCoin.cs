using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoin : Collectable
    {
        Vector2 direction = Vector2.one;
        bool stop = false;

        private void OnTriggerEnter2D(Collider2D coll)
        {
            PlayerDataHolder player = coll.gameObject.GetComponent<PlayerDataHolder>();

            if (player)// && coll.Equals(player.GetCharacterCapsuleCollider2D()))
                CmdUpdateServerState(false, coll.gameObject.GetComponent<PlayerDataHolder>().playerId);
        }

        private void Update()
        {
        }

        [Command]
        protected override void CmdUpdateServerState(bool b, int id)
        {
            base.CmdUpdateServerState(b, id);
            NetworkServer.UnSpawn(gameObject);
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