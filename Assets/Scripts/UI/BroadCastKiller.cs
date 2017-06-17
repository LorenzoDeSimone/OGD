using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Networking;
using UnityEngine.Networking;

public class BroadcastKiller : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(KillBroadcast());
    }

    private IEnumerator KillBroadcast()
    {
        yield return new WaitForSeconds(3);
        NetworkLobbyController.instance.SafeStopBroadCast();
    }
}
