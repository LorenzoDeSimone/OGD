using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace Assets.Scripts.Networking
{
	public class NetworkLobbyController : NetworkLobbyManager {

        bool loadingPublicMatches = true;
        List<MatchInfoSnapshot> publicMatches;
        MatchInfoSnapshot selectedMatchIfno;

        public void JoinPublicMatch()
        {
            StartMatchMaker();

            loadingPublicMatches = true;
            matchMaker.ListMatches(0, 5, "", false, 0, 0, InitMatchList);

            // gui coroutine here

            StartCoroutine(WaitAndJoinPublic());
        }

        private IEnumerator WaitAndJoinPublic()
        {
            yield return new WaitUntil(IsLoadingPublicMatches);

            if (publicMatches.Count == 0)
            {
                CreatePublicMatch();
            }
            else
            {
                //selectedMatchIfno = 
                //matchMaker.JoinMatch(networkID, "", "", "", 0, 0, OnMatchJoined);
                //backDelegate = lobbyManager.StopClientClbk;
                //_isMatchmaking = true;
                //DisplayIsConnecting();
            }
        }

        private void CreatePublicMatch()
        {
            CreateMatch(matchName);
        }

        public void InitMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            publicMatches = matches;
            loadingPublicMatches = false;
        }

        public void CreatePrivateMatch()
        {
            StartMatchMaker();
            CreateMatch("name");
        }

        public void JoinPrivateMatch()
        {
            StartMatchMaker();
            /*
             * if SearchAndJoin:
             *      show loading 
             * else
             *      show error
             */
        }

        private void CreateMatch(string matchName)
        {
            matchMaker.CreateMatch(
                   matchName,
                   (uint)maxPlayers,
                   true,
                   "", "", "", 0, 0,
                   OnMatchCreate);
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            //_currentMatchID = (System.UInt64)matchInfo.networkId;
        }

        private string RandomPublicName()
        {
            throw new NotImplementedException();
        }

        private bool IsLoadingPublicMatches()
        {
            return loadingPublicMatches;
        }
    }
}