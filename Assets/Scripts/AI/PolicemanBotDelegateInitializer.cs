using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;


public class PolicemanBotDelegateInitializer : NetworkBehaviour
{
    private ChaserBot myChaserBot;

    // Use this for initialization
    void Start ()
    {
        myChaserBot = GetComponent<ChaserBot>();
        myChaserBot.SetTargetGetter(GetBestPlayer);
        myChaserBot.SetOnHitHandler(PolicemanBotOnMissileHit);
        myChaserBot.SetOnCollectableHit(PolicemanBotOnCollectableHit);
    }

    public GameObject GetBestPlayer()
    {
        Dictionary<int, int> ofPlayersAndPoints = PointManager.instance.GetPointsForPlayers();
        int highestScore = -1;
        GameObject bestPlayer = null;

        foreach (int currPlayerID in ofPlayersAndPoints.Keys)
        {
            if(ofPlayersAndPoints[currPlayerID] > highestScore)
            {
                bestPlayer = myChaserBot.PlayersGameObjects[currPlayerID];
                highestScore = ofPlayersAndPoints[currPlayerID];
            }
        }
        return bestPlayer;
    }

    public void PolicemanBotOnMissileHit()
    {
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }

    public void PolicemanBotOnCollectableHit(GameObject collectable)
    {
       // Debug.LogError("oooo");
    }
}
