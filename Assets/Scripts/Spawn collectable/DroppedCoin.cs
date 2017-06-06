using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoin : Collectable
    {
        bool stop = false;
        bool onGround = false;
        Vector3 movementVersor;
        public float uncollectableTimeWindow = 0.5f;
        private bool isCollectable = false;
        
        private void Start()
        {
            Init();
            StartCoroutine(UncollectableTime(uncollectableTimeWindow));
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            PlayerDataHolder player = coll.gameObject.GetComponent<PlayerDataHolder>();

            if (isCollectable && player)// && coll.Equals(player.GetCharacterCapsuleCollider2D()))
                CmdUpdateServerState(false, coll.gameObject.GetComponent<PlayerDataHolder>().playerId);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            Platform gravityField = collision.collider.GetComponent<Platform>();
            if(gravityField!=null)
            {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Movable>().enabled = false;
                onGround = true;
            }
        }

        private void Update()
        {
            if (!onGround)
                transform.position = transform.position + movementVersor * GetComponent<Movable>().speed * Time.deltaTime;
        }

        public void SetMovementVersor(Vector3 movementVersor)
        {
            this.movementVersor = movementVersor;
        }

        [Command]
        private void CmdUpdateServerState(bool b, int id)
        {
            RealUpdate(b, id);
            NetworkServer.UnSpawn(gameObject);
            Destroy(gameObject);
        }

        IEnumerator<WaitForSeconds> UncollectableTime(float uncollectableTime)
        {
            yield return new WaitForSeconds(uncollectableTime);
            isCollectable = true;
        }

    }

}