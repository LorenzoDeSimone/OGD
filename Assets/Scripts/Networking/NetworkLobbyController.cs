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
        internal PlayMenu currentPlayMenu;

        public delegate void OnPlayerDisconnectDelegate(NetworkConnection conn, int playerCount);
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
                Debug.LogWarning("Create and join at "+matchInfo.networkId);
            }
            else
            {
                Debug.LogError("Create fail "+matchInfo);
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
                Debug.LogWarning("Destoyed "+extendedInfo);
            }
            else
            {
                Debug.LogError("Destroy Fail "+extendedInfo);
            }

            readyToReset = true;
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            Debug.LogWarning("Player dsconected! " + conn);

            if (PlayerDisconnectEvent!= null)
            {
                // num players is an inherited field 
                PlayerDisconnectEvent(conn,numPlayers);
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

        public void PrepareToReset()
        {
            if(createdMatchID != (ulong)NetworkID.Invalid && matchMaker)
            {
                matchMaker.DestroyMatch((NetworkID)createdMatchID, 0, OnDestroyMatch);
                createdMatchID = (ulong)NetworkID.Invalid;
            }
            else
            {
                readyToReset = true;
            }
        }

        public void ResetNetworkState()
        {
            StopAllCoroutines();

            if(currentPlayMenu)
                currentPlayMenu.StopMatchSearch();

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