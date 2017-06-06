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
        public string controlsHolderTag = "ControlsHolder";

        public RectTransform scoresHolder;
        public GameObject playerScorePrefab;
        public PlayerDresser dresser;

        private NetworkLobbyController lobbyController;

        void Start()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            
            GameObject.FindGameObjectWithTag(controlsHolderTag).SetActive(false);
            FillScoreboard();
            StartCoroutine(PrepareToDisconnect());
        }

        private void FillScoreboard()
        {
            Dictionary<int, int> ofPlayersAndPoints = PointManager.instance.GetPointsForPlayers();
            float size = 1 / (float)ofPlayersAndPoints.Keys.Count;
            float offset;
            GameObject go;
            PlayerScore ps;

            foreach (int i in ofPlayersAndPoints.Keys)
            {
                go = Instantiate(playerScorePrefab, scoresHolder);
                ps = go.GetComponent<PlayerScore>();
                ps.SetSprite(dresser.GetSprite(i));
                ps.SetPoints(ofPlayersAndPoints[i], PointManager.instance.GetTotalPoints());
                offset = ((float)PointManager.instance.GetPlayerRankPosition(i)-1) * size;
                ((RectTransform)ps.transform).anchorMin.Set(offset,0);
                ((RectTransform)ps.transform).anchorMax.Set(offset+size,1);
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
            MasterServer.UnregisterHost();
            lobbyController.ResetNetworkState();
            lobbyController.ServerReturnToLobby();
        }
    }
}
