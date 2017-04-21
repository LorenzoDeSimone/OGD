using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Events;
namespace Assets.Scripts.Networking
{
    public class NetworkLobbyController : NetworkLobbyManager {

        public float publicMatchWaitTime = 10.0f;

        internal bool loadingPublicMatches = true;
        internal bool readyToReset = true;
        internal bool creatingMatch = true;
        internal bool joiningMatch = false;
        internal bool searchingPublicMatch = true;

        internal List<MatchInfoSnapshot> publicMatches;

        public delegate void OnPlayerDisconnectDelegate(NetworkPlayer player, int playerCount);
        public static event OnPlayerDisconnectDelegate PlayerDisconnectEvent;

        ulong netId;
        ulong nodeId;
        ulong createdMatchID = (ulong)NetworkID.Invalid;

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            
            if (success)
            {
                netId = (ulong)matchInfo.networkId;
                nodeId = (ulong)matchInfo.nodeId;
                createdMatchID = (ulong)matchInfo.networkId;
                searchingPublicMatch = false;
                Debug.Log("Create and join at "+matchInfo.networkId);
            }
            else
            {
                Debug.LogError("Create fail");
            }

            creatingMatch = false;
        }

        public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchJoined(success, extendedInfo, matchInfo);
            searchingPublicMatch = !success;

            if (success)
            {
                netId = (ulong)matchInfo.networkId;
                nodeId = (ulong)matchInfo.nodeId;
                Debug.Log("Join at "+matchInfo.networkId);
            }
            else
            {
                Debug.LogError("Join fail");
            }

            joiningMatch = false;
        }

        public override void OnDestroyMatch(bool success, string extendedInfo)
        {
            base.OnDestroyMatch(success, extendedInfo);
            if(success)
            {
                Debug.Log("Destoyed");
            }
            else
            {
                Debug.LogError("Destroy Fail");
            }
            Stop();
        }

        private void OnPlayerDisconnected(NetworkPlayer player)
        {
            if(PlayerDisconnectEvent!= null)
            {
                PlayerDisconnectEvent(player,numPlayers);
            }
        }

        public void CreateMatch(string matchName)
        {
            matchMaker.CreateMatch(
                   matchName,
                   (uint)maxPlayers,
                   true,
                   "", "", "", 0, 0,
                   OnMatchCreate);
        }

        public void InitMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
        {
            if (success)
            {
                publicMatches = matches;
            }
            else
            {
                publicMatches = new List<MatchInfoSnapshot>();
            }

            loadingPublicMatches = false;
        }

        public void ResetAndStop()
        {
            StopAllCoroutines();

            if(createdMatchID != (ulong)NetworkID.Invalid)
            {
                matchMaker.DestroyMatch((NetworkID)createdMatchID, 0, OnDestroyMatch);
                createdMatchID = (ulong)NetworkID.Invalid;
            }
            else
            {
                Stop();
            }
        }

        private void Stop()
        {
            readyToReset = true;

            if (matchMaker)
            {
                StopMatchMaker();
                StopHost();
                StopClient();
            }

            loadingPublicMatches = true;
            searchingPublicMatch = true;
            joiningMatch = false;
            creatingMatch = true;
        }

        public NetworkPlayer GetLocalPlayer()
        {
            foreach( UnityEngine.Networking.PlayerController pC in singleton.client.connection.playerControllers )
            {
                if (pC.unetView.isLocalPlayer)
                    return pC.unetView.GetComponent<NetworkPlayer>();
            }

            return new NetworkPlayer();
        }

        public bool IsSearchingPublicMatch()
        {
            return searchingPublicMatch;
        }

        public bool IsLoadingPublicMatches()
        {
            return loadingPublicMatches;
        }

        public bool IsJoiningMatch()
        {
            return joiningMatch;
        }

        public bool IsCreatingMatch()
        {
            return creatingMatch;
        }

        public bool IsReadyToReset()
        {
            return readyToReset;
        }
    }
}