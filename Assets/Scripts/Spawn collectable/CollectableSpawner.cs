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
        public GameObject missilePrefab;
        public GameObject countdown;

        public bool spawnNpc = true;
        public int minCountdown = 10;
        public int maxCountdown = 20;

        private HashSet<GameObject> collectables;
        private List<Vector3> positions;
        private CountDown countdownCounter;
        private Platform parentPlatform;

        private void Start()
        {
            GameObject go;
            parentPlatform = transform.parent.GetComponent<Platform>();
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
            StartCoroutine(FirstCountdown());
        }

        private void InitCollectables()
        {
            GameObject go;
            foreach (Transform t in transform)
            {
                if (t.gameObject.tag == "SpawnPointCoin")
                {
                    go = Instantiate(collectablePrefab, t.position, Quaternion.identity, transform);
                    collectables.Add(go);
                    NetworkServer.Spawn(go);
                    positions.Add(t.position);
                }
                else if (t.gameObject.tag == "SpawnPointMissile")
                {
                    go = Instantiate(missilePrefab, t.position, Quaternion.identity, transform);
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
            int i, j;
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

        private IEnumerator FirstCountdown()
        {
            InitCollectables();
            yield return new WaitForSecondsRealtime(3);
            countdownCounter.RpcChangeNetworkState(0);
            StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            int countdown = Random.Range(minCountdown, maxCountdown + 1);
            countdownCounter.RpcChangeNetworkState(countdown);
            yield return new WaitForSecondsRealtime(countdown);
            SpawnCoins();
            if (spawnNpc && parentPlatform.players.Count == 0)
                SpawnNpc();
            yield return new WaitForSecondsRealtime(3);
            StartCoroutine(StartCountdown());
        }

        private void SpawnNpc()
        {
            GameObject npcPrefab = NpcSpawner.Instance.getNpc();
            if (npcPrefab != null)
            {
                NpcSpawner.Instance.addNpc();
                Transform platfomTransform = transform.parent;
                GameObject go = Instantiate(npcPrefab, platfomTransform.position + platfomTransform.up + npcPrefab.transform.up, Quaternion.identity, transform);
                NetworkServer.Spawn(go);
            }
        }
    }
}
