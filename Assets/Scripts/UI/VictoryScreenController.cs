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
            PlayerDataHolder playerData;
            GameObject newPlayerScore;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            int totalPoints = 0;
            int playerPoints = 0;
            float offset = 0.0f;
            Vector2 newAnchorMax;
            Vector2 newAnchorMin;

            foreach (GameObject go in players)
            {
                playerData = go.GetComponent<PlayerDataHolder>();
                totalPoints += playerData.GetPoints();
            }

            for(int i = 0; i < players.Length; i++)
            {
                newPlayerScore = Instantiate(playerScorePrefab, scoresHolder);
                playerData = players[i].GetComponent<PlayerDataHolder>();
                newPlayerScore.GetComponent<Image>().color = PlayerColor.GetColor(playerData.GetPlayerNetworkId());

                newAnchorMax = ((RectTransform)newPlayerScore.transform).anchorMax;
                newAnchorMin = ((RectTransform)newPlayerScore.transform).anchorMin;
                newAnchorMin.x = offset;
                newAnchorMax.x = offset + 1/(float)players.Length;
                offset = newAnchorMax.x;

                if (totalPoints > 0)
                {
                    playerPoints = playerData.GetPoints();

                    if (playerPoints <= 0)
                    {
                        newAnchorMax.y = 0.1f;
                    }
                    else
                    {
                        newAnchorMax.y = playerPoints/ (float)totalPoints;
                    }
                }
                else
                {
                    newAnchorMax.y = 1 / (float)players.Length;
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
            Network.Disconnect();
            lobbyController.ResetNetworkState();
        }
    }
}
