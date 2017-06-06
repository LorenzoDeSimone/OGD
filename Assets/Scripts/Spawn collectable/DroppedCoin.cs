using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoin : Collectable
    {
        bool stop = false;

        private void Start()
        {
            Init();
        }

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
        private void CmdUpdateServerState(bool b, int id)
        {
            RealUpdate(b, id);
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }

        public void Stop()
        {
            stop = true;
        }
    }

}