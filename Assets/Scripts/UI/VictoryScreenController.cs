using Assets.Scripts.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using Assets.Scripts.Player;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    class VictoryScreenController : NetworkBehaviour
    {
        [Header("Time of victory screen in seconds")]
        public float vicotoryScreenTime = 4;

        public string poinbarHolderTag = "PointBarHolder";

        public RectTransform scoresHolder;
        public GameObject playerScorePrefab;

        private NetworkLobbyController lobbyController;

        void Start()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            FillScoreboard();
            StartCoroutine(PrepareToDisconnect());
        }

        /*
         * May Lorenzo forgive me...
         */
        private void FillScoreboard()
        {
            PointManager pointManager = GameObject.FindGameObjectWithTag(poinbarHolderTag).GetComponent<PointManager>();
            GameObject newPlayerScore;
            Dictionary<int, int> ofPlayersAndPoints = pointManager.GetPointsForPlayers();
            float offset = 0.0f;
            Vector2 newAnchorMax;
            Vector2 newAnchorMin;

            foreach(int i in ofPlayersAndPoints.Keys)
            {
                newPlayerScore = Instantiate(playerScorePrefab, scoresHolder);

                newPlayerScore.GetComponent<Image>().color = PlayerColor.GetColor(ofPlayersAndPoints[i]);

                newAnchorMax = ((RectTransform)newPlayerScore.transform).anchorMax;
                newAnchorMin = ((RectTransform)newPlayerScore.transform).anchorMin;
                newAnchorMin.x = offset;
                newAnchorMax.x = offset + 1/(float)ofPlayersAndPoints.Count;
                offset = newAnchorMax.x;

                if (pointManager.GetTotalPoints() > 0)
                {

                    if (ofPlayersAndPoints[i] <= 0)
                    {
                        newAnchorMax.y = 0.1f;
                    }
                    else
                    {
                        newAnchorMax.y = ofPlayersAndPoints[i] / (float)pointManager.GetTotalPoints();
                    }
                }
                else
                {
                    newAnchorMax.y = 1 / (float)ofPlayersAndPoints.Count;
                }
                ((RectTransform)newPlayerScore.transform).anchorMax = newAnchorMax;
                ((RectTransform)newPlayerScore.transform).anchorMin = newAnchorMin;
            }
        }

        private IEnumerator PrepareToDisconnect()
        {
            lobbyController.PrepareToReset();
            yield return new WaitForSeconds(vicotoryScreenTime);

            DisconnectFromMatch();
        }

        private void DisconnectFromMatch()
        {
            Debug.LogWarning("Disconnection...");
            if(isServer)
            {
                MasterServer.UnregisterHost();
            }
            Network.Disconnect();
            lobbyController.ResetNetworkState();
        }
    }
}
