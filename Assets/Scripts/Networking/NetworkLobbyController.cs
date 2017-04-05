using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

namespace Assets.Scripts.Networking
{
	public class NetworkLobbyController : NetworkLobbyManager {

        const string CHARS_POOL = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789-";
        const int NAME_LENGHT = 16;

        public float publicMatchWaitTime = 10.0f;

        bool loadingPublicMatches = true;
        bool searchingPublicMatch = true;
        bool joiningMatch = false;
        bool creatingMatch = true;

        List<MatchInfoSnapshot> publicMatches;

        public void JoinPublicMatch()
        {
            StartMatchMaker();

            // gui coroutine here

            StartCoroutine(JoinPublic());
        }

        private IEnumerator JoinPublic()
        {
            while(searchingPublicMatch)
            {
                loadingPublicMatches = true;
                matchMaker.ListMatches(0, 10, "", false, 0, 0, InitMatchList);
                yield return new WaitWhile(IsLoadingPublicMatches);

                Debug.Log(publicMatches);

                if (publicMatches.Count == 0)
                {
                    creatingMatch = true;
                    CreatePublicMatch();
                    yield return new WaitWhile(IsCreatingMatch);
                }
                else
                {
                    foreach(MatchInfoSnapshot mis in publicMatches)
                    {
                        joiningMatch = true; 
                        matchMaker.JoinMatch(mis.networkId, "", "", "", 0, 0, OnMatchJoined);
                        yield return new WaitWhile(IsJoiningMatch);

                        if (!searchingPublicMatch)
                            break;
                    }
                }
            }
        }

        private void CreatePublicMatch()
        {
            CreateMatch( RandomPublicName() );
        }

        private void InitMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if(success)
            {
                publicMatches = matches;
            }
            else
            {
                publicMatches = new List<MatchInfoSnapshot>();
            }

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

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);

            Debug.Log("Create:"+success);

            if (success)
            {
                matchMaker.JoinMatch(matchInfo.networkId, "", "", "", 0, 0, OnMatchJoined);
            }

            creatingMatch = false;
        }

        public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchJoined(success, extendedInfo, matchInfo);
            searchingPublicMatch = !success;
            joiningMatch = false;
            Debug.Log("Join:"+success);
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

        private string RandomPublicName()
        {
            string randName = "";
            System.Random rInt = new System.Random();

            for ( int i = 0; i < NAME_LENGHT; i++ )
            {
                randName += CHARS_POOL[rInt.Next()%CHARS_POOL.Length];
            }

            Debug.Log(randName);
            return randName;
        }

        private bool IsLoadingPublicMatches()
        {
            return loadingPublicMatches;
        }

        private bool IsJoiningMatch()
        {
            return joiningMatch;
        }

        private bool IsCreatingMatch()
        {
            return creatingMatch;
        }

        public void ResetAndStop()
        {
            StopClient();
            StopMatchMaker();
            StopAllCoroutines();
            loadingPublicMatches = true;
            searchingPublicMatch = true;
            joiningMatch = false;
            Debug.Log("Reset");
        }
    }
}