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
            Vector2 temp = new Vector2();

            foreach (int i in ofPlayersAndPoints.Keys)
            {
                go = Instantiate(playerScorePrefab, scoresHolder);
                ps = go.GetComponent<PlayerScore>();
                ps.SetSprites(dresser.GetSprite(i),PointManager.instance.GetPointBar(i));
                ps.SetPoints(ofPlayersAndPoints[i], PointManager.instance.GetTotalPoints());
                offset = (PointManager.instance.GetMatchSize() - PointManager.instance.GetPlayerRankPosition(i)) * size;
                temp.Set(0, offset);
                ((RectTransform)ps.transform).anchorMin = temp;
                temp.Set(1, offset+size);
                ((RectTransform)ps.transform).anchorMax = temp;
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
