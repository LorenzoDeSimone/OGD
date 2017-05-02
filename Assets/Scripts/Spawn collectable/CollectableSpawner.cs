using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Spawn_collectable
{
    class CollectableSpawner : NetworkBehaviour
    {
        public GameObject collectablePrefab;
        public float spawnTime = 5.0f;
        public float bias = 0.0f;

        private List<Transform> spawnPositions;
        private HashSet<GameObject> collectables;

        private void Start()
        {
            collectables = new HashSet<GameObject>();
            spawnPositions = new List<Transform>(GetComponentsInChildren<Transform>());
            //remove my tranform
            spawnPositions.Remove(transform);
            InitCollectables();
            InvokeRepeating("SpawnCoins", 2.0f, spawnTime);
        }

        private void InitCollectables()
        {
            GameObject go;
            foreach( Transform t in spawnPositions )
            {
                go = Instantiate(collectablePrefab, t.position, Quaternion.identity);
                collectables.Add(go);
                NetworkServer.Spawn(go);
            }
        }
        
        private void SpawnCoins()
        {
            foreach( GameObject g in collectables )
            {
                g.SetActive(true);
            }
        }
    }
}
