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
    
    [Server]
    void Start()
    {
        npcPrefabChosen = null;
        numNpc = 0;
        StartCoroutine(NextSpawn());
    }

    public IEnumerator NextSpawn()
    {
        yield return new WaitForSeconds(Random.Range(minTime, maxTime + 1));
        if(numNpc < numNpcMax)
            npcPrefabChosen = npcPrefabs[Random.Range(0, npcPrefabs.Count)];
    }

    public GameObject getNpc()
    {
        GameObject go = npcPrefabChosen;
        if (npcPrefabChosen != null)
        {
            numNpc++;
            npcPrefabChosen = null;
            StartCoroutine(NextSpawn());
        }
        return go;
    }

    public void removeNpc()
    {
        numNpc--;
    }
}
