﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Assets.Scripts.Spawn_collectable;
using UnityEngine;
using UnityEngine.Networking;

public class GoldMagnetBotDelegateInitializer : NetworkBehaviour
{
    [SyncVar]
    int coinsCollected;

    // Use this for initialization
    void Start()
    {
        ChaserBot myChaserBot = GetComponent<ChaserBot>();
        myChaserBot.SetTargetGetter(GetWorstPlayer);
        myChaserBot.SetOnHitHandler(GoldMagnetBotOnMissileHit);
        myChaserBot.SetOnCollectableHit(GoldMagnetBotOnCollectableHit);
        coinsCollected = 0;
    }

    public GameObject GetWorstPlayer()
    {
        Dictionary<int, int> ofPlayersAndPoints = PointManager.instance.GetPointsForPlayers();
        int lowestScore = int.MaxValue;
        GameObject worstPlayer = null;

        foreach (int currPlayerID in ofPlayersAndPoints.Keys)
        {
            if (ofPlayersAndPoints[currPlayerID] < lowestScore)
            {
                worstPlayer = ChaserBot.PlayersGameObjects[currPlayerID];
                lowestScore = ofPlayersAndPoints[currPlayerID];
            }
        }

        return worstPlayer;
    }

    public void GoldMagnetBotOnMissileHit()
    {
        GetComponent<CoinDropper>().DropCoins(coinsCollected);
        NetworkServer.UnSpawn(gameObject);
        Destroy(gameObject);
    }

    public void GoldMagnetBotOnCollectableHit(GameObject collectable)
    {
        Collectable collectableComponent = collectable.GetComponent<Collectable>();

        if (collectableComponent)
            coinsCollected += collectableComponent.pointValue;

    }
}