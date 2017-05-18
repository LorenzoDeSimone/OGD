using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Spawn_collectable
{
    class CollectableSpawner : NetworkBehaviour
    {
        public GameObject collectablePrefab;
        public GameObject countdown;

        public int minCountdown = 10;
        public int maxCountdown = 20;

        private HashSet<GameObject> collectables;
        private CountDown countdownCounter;

        private void Start()
        {
            collectables = new HashSet<GameObject>();
            GameObject go = Instantiate(countdown, transform.position, Quaternion.identity, transform);
            NetworkServer.Spawn(go);
            countdownCounter = go.GetComponent<CountDown>();
            StartCoroutine(firstCountdown());
        }

        private void InitCollectables()
        {
            GameObject go;
            foreach ( Transform t in transform )
            {
                if(t.gameObject.tag == "SpawnPoint")
                {
                    go = Instantiate(collectablePrefab, t.position, Quaternion.identity, transform);
                    collectables.Add(go);
                    NetworkServer.Spawn(go);
                }
            }
        }

        private void SpawnCoins()
        {
            Collectable c;
            foreach (GameObject g in collectables)
            {
                c = g.GetComponent<Collectable>();
                if (!c.GetNetworkActiveState())
                    c.RpcChangeNetworkState(true);
            }
        }

        private IEnumerator firstCountdown()
        {
            countdownCounter.RpcChangeNetworkState("3");
            yield return new WaitForSeconds(1);
            countdownCounter.RpcChangeNetworkState("2");
            yield return new WaitForSeconds(1);
            countdownCounter.RpcChangeNetworkState("1");
            yield return new WaitForSeconds(1);
            countdownCounter.RpcChangeNetworkState("");
            InitCollectables();  // Isn't on start! is it a problem?
            yield return new WaitForSeconds(3);
            StartCoroutine(startCountdown());
        }

        private IEnumerator startCountdown()
        {
            int i = Random.Range(minCountdown, maxCountdown + 1);
            //wait countdown
            while (i > 0)
            {
                countdownCounter.RpcChangeNetworkState("" + i);
                i--;
                yield return new WaitForSeconds(1);
            }
            countdownCounter.RpcChangeNetworkState("");
            SpawnCoins();
            yield return new WaitForSeconds(3);
            StartCoroutine(startCountdown());
        }
    }
}
