using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using Assets.Scripts.Networking;
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
        int highestScore = int.MinValue;
        GameObject bestPlayer = null;
        PlayerDataHolder currPlayerDataHolder;

        foreach (GameObject currPlayer in NetworkLobbyController.instance.GetPlayersInMatch())
        {
            currPlayerDataHolder = currPlayer.GetComponent<PlayerDataHolder>();

            if (currPlayerDataHolder.GetPoints() > highestScore)
            {
                bestPlayer = currPlayer;
                highestScore = currPlayerDataHolder.GetPoints();
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
