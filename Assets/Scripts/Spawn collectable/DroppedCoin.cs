using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Assets.Scripts.Spawn_collectable
{
    public class DroppedCoin : Collectable
    {
        bool stop = false;
        //bool onGround = false;
        [SyncVar]
        public Vector3 startPoint, airPoint, groundPoint;
        public float uncollectableTimeWindow = 0.5f;
        private bool isCollectable = false;
        private float time=0f;
        public float timeToReachGround = 2f;

        private void Start()
        {
            Init();
            StartCoroutine(UncollectableTime(uncollectableTimeWindow));
        }

        private void OnTriggerStay2D(Collider2D coll)
        {
            PlayerDataHolder player = coll.gameObject.GetComponent<PlayerDataHolder>();
            if (isCollectable && player)// && coll.Equals(player.GetCharacterCapsuleCollider2D()))
                UpdateServerState(false, coll.gameObject.GetComponent<PlayerDataHolder>().playerId);
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            Platform platform = coll.gameObject.GetComponent<Platform>();
            //if (platform)
            //    onGround = true;
        }

        private void Update()
        {
            if (time <= timeToReachGround)
            {
                time += Time.deltaTime;
                float normalizedTime = time / timeToReachGround;

                transform.position = Mathf.Pow((1 - normalizedTime), 2) * startPoint +
                                     2 * (1 - normalizedTime) * normalizedTime * airPoint +
                                     Mathf.Pow(normalizedTime, 2) * groundPoint; 
            }
        }

        public void SetCurvePoints(Vector3 startPoint, Vector3 airPoint, Vector3 groundPoint)
        {
            this.startPoint = startPoint;
            this.airPoint = airPoint;
            this.groundPoint = groundPoint;
        }

        private void UpdateServerState(bool b, int id)
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