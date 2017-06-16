using System;
using System.Collections;
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

#if UNITY_STANDALONE_WINDOWS
            isHostingMatch = true;
#endif

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

                    foreach( NetworkBroadcastResult nbs in networkExplorer.broadcastsReceived.Values)
                    {
                        lobbyController.networkAddress = nbs.serverAddress;

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
                        }
                        if (startdAsClient)
                            break;
                    }

                }
                yield return new WaitForSeconds(0.1f);
            }
            while (!startdAsClient);
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
