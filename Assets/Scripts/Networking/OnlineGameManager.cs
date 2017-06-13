using System.Collections;
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
            timer = Instantiate(timer);
            timer.GetComponent<TimeManager>().SetEndTime(matchTime);
            NetworkServer.Spawn(timer);
            StartCoroutine(StartMatchCountDown());
        }

        private IEnumerator StartMatchCountDown()
        {
            yield return new WaitForSecondsRealtime(10);
            timer.GetComponent<TimeManager>().SetEndTime(matchTime - 10);
            yield return new WaitForSecondsRealtime(matchTime - 10);
            EndMatch();
        }

        public void EndMatch()
        {
            GameObject go = Instantiate(victoryScreenHolder);
            NetworkServer.Spawn(go);
        }
    }
}
