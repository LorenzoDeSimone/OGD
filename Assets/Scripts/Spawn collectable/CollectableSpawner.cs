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
        private List<Vector3> positions;
        private CountDown countdownCounter;

        private void Start()
        {
            GameObject go;
            collectables = new HashSet<GameObject>();
            positions = new List<Vector3>();
            try
            {
                go = Instantiate(countdown, transform.position, Quaternion.identity, transform);
                NetworkServer.Spawn(go);
            }
            catch (System.Exception)
            {
                go = Instantiate((GameObject)Resources.Load("Prefabs/Platforms/Counter"), transform.position, Quaternion.identity, transform);
                NetworkServer.Spawn(go);
            }
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
                    positions.Add(t.position);
                }
            }
            SpawnCoins();
        }

        private void SpawnCoins()
        {
            Collectable c;
            Vector3 temp;
            int i,j;
            for (i = 0; i < positions.Count - 1; i++)
            {
                j = Random.Range(0, positions.Count);
                temp = positions[i];
                positions[i] = positions[j];
                positions[j] = temp;
            }
            i = 0;
            foreach (GameObject g in collectables)
            {
                c = g.GetComponent<Collectable>();
                if (!c.GetNetworkActiveState())
                    c.RpcChangeNetworkState(true, positions[i]);
                i++;
            }
        }

        private IEnumerator firstCountdown()
        {
            InitCollectables();
            yield return new WaitForSecondsRealtime(3);
            countdownCounter.RpcChangeNetworkState(0);
            StartCoroutine(startCountdown());
        }

        private IEnumerator startCountdown()
        {
            int countdown = Random.Range(minCountdown, maxCountdown + 1);
            countdownCounter.RpcChangeNetworkState(countdown);
            yield return new WaitForSecondsRealtime(countdown);
            SpawnCoins();
            yield return new WaitForSecondsRealtime(3);
            StartCoroutine(startCountdown());
        }
    }
}
