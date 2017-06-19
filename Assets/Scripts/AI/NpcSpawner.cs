using Assets.Scripts.Spawn_collectable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NpcSpawner : NetworkBehaviour
{
    public List<GameObject> npcPrefabs;
    public int minTime = 30, maxTime = 30, numNpcMax = 3;

    private  GameObject npcPrefabChosen;
    private List<GameObject> platforms;
    private int numNpc;
    private float nextSpawnTime;

    private static NpcSpawner instance;
    
    public static NpcSpawner Instance
    {
        get
        {
            if (instance == null)
                instance = new NpcSpawner();
            return instance;
        }
    }

    [Server]
    void Start()
    {
        npcPrefabChosen = null;
        numNpc = 0;
        nextSpawnTime = Time.time + Random.Range(minTime, maxTime + 1);
        instance = this;
    }

    public GameObject getNpc()
    {
        if(Time.time > nextSpawnTime && numNpc < numNpcMax)
        {
            nextSpawnTime = Time.time + Random.Range(minTime, maxTime + 1);
            return npcPrefabs[Random.Range(0, npcPrefabs.Count)];
        }
        return null;
    }

    public void addNpc()
    {
        numNpc++;
    }

    public void removeNpc()
    {
        numNpc--;
    }
}
