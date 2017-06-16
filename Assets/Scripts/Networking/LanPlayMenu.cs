using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class LanPlayMenu : PlayMenu
    {
        public bool isHostingMatch = false;

        NetworkDiscovery networkExplorer;
        bool startdAsClient = false;
        bool startedAsServer = false;

        protected override void InitMenu()
        {
            networkExplorer = lobbyController.GetNetExplorer();

            TryJoinMatch();
            StartCoroutine(SpinLoadingSpinner());
        }

        protected override void TryJoinMatch()
        {
            lobbyController.readyToReset = false;

            if (isHostingMatch)
            {
                StartCoroutine(RunServer());
            }
            else
            {
                StartCoroutine(RunClient());
            }

        }

        private IEnumerator RunServer()
        {
            try
            {
                lobbyController.StartHost();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                startedAsServer = true;
            }

            if (startedAsServer)
            {
                networkExplorer.Initialize();
                yield return new WaitForSeconds(0.1f);
                networkExplorer.StartAsServer();
            }
        }

        private IEnumerator RunClient()
        {
            networkExplorer.Initialize();
            yield return new WaitForSeconds(0.1f);
            networkExplorer.StartAsClient();

            do
            {
                if (networkExplorer == null || networkExplorer.broadcastsReceived == null)
                {
                    continue;
                }

                if (networkExplorer.broadcastsReceived.Count > 0)
                {
                    //Last element is always the new one
                    lobbyController.networkAddress = GetLastOf(networkExplorer.broadcastsReceived);

                    try
                    {
                        lobbyController.StartClient();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                    finally
                    {
                        startdAsClient = true;
                        SafeStopBroadcast();
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
            while (!startdAsClient);
        }

        private string GetLastOf(Dictionary<string, NetworkBroadcastResult> dict)
        {
            string resKey = "";
            foreach ( string s in dict.Keys )
            {
                resKey = s;
            }
            return dict[resKey].serverAddress;
        }

        private void SafeStopBroadcast()
        {
            if (networkExplorer.running)
                networkExplorer.StopBroadcast();
        }

        protected override void TryInitMenu()
        {
            if (!lobbyController.Online)
            {
                lobbyController.currentPlayMenu = this;
                InitMenu();
            }
        }
    }
}
