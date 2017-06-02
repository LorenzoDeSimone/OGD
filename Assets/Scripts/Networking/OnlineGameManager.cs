﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Assets.Scripts.Player;

namespace Assets.Scripts.Networking
{
    class OnlineGameManager : NetworkBehaviour
    {
        [Header("Time of a match in seconds")]
        public float matchTime = 180;
        public GameObject timer;

        public GameObject victoryScreenHolder;

        void Start()
        {
            GameObject go = Instantiate(timer);
            NetworkServer.Spawn(go);
            go.GetComponent<TimeManager>().RpcSetEndTime(Time.time + matchTime);
            StartCoroutine(StartMatchCountDown());
        }

        private IEnumerator StartMatchCountDown()
        {
            yield return new WaitForSecondsRealtime(matchTime);
            EndMatch();
        }

        public void EndMatch()
        {
            GameObject go = Instantiate(victoryScreenHolder);
            NetworkServer.Spawn(go);
        }
    }
}
