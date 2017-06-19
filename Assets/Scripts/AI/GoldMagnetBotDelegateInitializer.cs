using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Assets.Scripts.Spawn_collectable;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;

public class GoldMagnetBotDelegateInitializer : NetworkBehaviour
{
    private ChaserBot myChaserBot;

    [SyncVar]
    int coinsCollected;

    // Use this for initialization
    void Start()
    {
        myChaserBot = GetComponent<ChaserBot>();
        myChaserBot.SetTargetGetter(GetWorstPlayer);
        myChaserBot.SetOnHitHandler(GoldMagnetBotOnMissileHit);
        myChaserBot.SetOnCollectableHit(GoldMagnetBotOnCollectableHit);
        coinsCollected = 0;
    }

    public GameObject GetWorstPlayer()
    {
        int lowestScore = int.MaxValue;
        GameObject worstPlayer = null;
        PlayerDataHolder currPlayerDataHolder;

        foreach (GameObject currPlayer  in NetworkLobbyController.instance.GetPlayersInMatch())
        {
            currPlayerDataHolder = currPlayer.GetComponent<PlayerDataHolder>();

            if ( currPlayerDataHolder.GetPoints() < lowestScore)
            {
                worstPlayer = currPlayer;
                lowestScore = currPlayerDataHolder.GetPoints();
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
