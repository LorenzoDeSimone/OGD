using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Networking;
using UnityEngine.Networking;

public class BroadCastKiller : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(KillBroadCast());
    }

    private IEnumerator KillBroadCast()
    {
        yield return new WaitForSeconds(3);
        NetworkLobbyController.instance.SafeStopBroadCast();
    }
}
