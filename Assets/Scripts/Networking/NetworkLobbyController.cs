using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.Events;
using Assets.Scripts.Player;

namespace Assets.Scripts.Networking
{
    public class NetworkLobbyController : NetworkLobbyManager {

        public float publicMatchWaitTime = 10.0f;
        public static NetworkLobbyController instance;

        internal bool loadingMatches = true;
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
        bool online = true;
        NetworkDiscovery networkExplorer;
        //Gives players a unique id
        int idCounter = 0;

        private void Awake()
        {
            if (instance == null)
                instance = this;
        }

        private void Start()
        {
            networkExplorer = GetComponent<NetworkDiscovery>();
        }

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
                Debug.LogWarning("Join fail");
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
                Debug.LogWarning("Destroy Fail "+extendedInfo);
            }

            readyToReset = true;
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            try
            {
                base.OnServerDisconnect(conn);
                Debug.LogWarning("Player dsconected! " + conn);

                if (PlayerDisconnectEvent != null)
                {
                    // num players is an inherited field 
                    PlayerDisconnectEvent(conn, numPlayers);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Player dsconnection error " + e.Message);
            }
        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            bool ret =  base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
            gamePlayer.GetComponent<PlayerDataHolder>().playerId = idCounter;
            idCounter += 1;

            //reset player id
            if (idCounter == maxPlayers)
                idCounter = 0;

            return ret;
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

            loadingMatches = false;
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

            SafeStopBroadCast();

            if (currentPlayMenu)
                currentPlayMenu.StopMatchSearch();

            if (matchMaker)
            {
                StopMatchMaker();
            }

            StopClient();
            StopHost();

            loadingMatches = true;
            searchingPublicMatch = true;
            joiningMatch = false;
            creatingMatch = true;
        }

        public void SafeStopBroadCast()
        {
            if (networkExplorer && networkExplorer.running)
                networkExplorer.StopBroadcast();
        }

        public bool Online
        {
            get
            {
                return online;
            }

            set
            {
                online = value;
            }
        }

        public NetworkDiscovery GetNetExplorer()
        {
            return networkExplorer;
        }

        public bool IsSearchingPublicMatch()
        {
            return searchingPublicMatch;
        }

        public bool IsLoadingPublicMatches()
        {
            return loadingMatches;
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