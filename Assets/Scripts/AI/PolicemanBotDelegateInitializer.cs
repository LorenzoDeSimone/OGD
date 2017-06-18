using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;


public class PolicemanBotDelegateInitializer : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
    {
        GetComponent<ChaserBot>().SetTargetGetter(GetBestPlayer);
        GetComponent<ChaserBot>().SetOnHitHandler(PolicemanBotOnMissileHit);
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
                bestPlayer = ChaserBot.PlayersGameObjects[currPlayerID];
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

}
