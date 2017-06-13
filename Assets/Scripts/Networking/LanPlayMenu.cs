using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    class LanPlayMenu : PlayMenu
    {
        NetworkDiscovery networkExplorer;

        public float waitTime = 0.1f;
        [Header("Base connection attempts value, a random bias vill be added")]
        public int attempts = 5;

        protected override void InitMenu()
        {
            networkExplorer = lobbyController.GetNetExplorer();

            TryJoinMatch();
            StartCoroutine(SpinLoadingSpinner());
        }

        protected override void TryJoinMatch()
        {
            lobbyController.readyToReset = false;
            StartCoroutine(SearchMatch());
        }


        protected override IEnumerator SearchMatch()
        {
            bool startdAsClient = false;
            bool startedAsServer = false;
            bool restart = false;

            do
            {
                Debug.LogWarning("Searching");
                SafeStopBroadcast();
                yield return new WaitForSeconds(0.1f);
                networkExplorer.Initialize();
                yield return new WaitForSeconds(0.1f);
                networkExplorer.StartAsClient();

                for (int i = 0; i < GetRandomAttempts(); i++)
                {
                    if (networkExplorer == null || networkExplorer.broadcastsReceived == null)
                    {
                        continue;
                    }

                    if (networkExplorer.broadcastsReceived.Count > 0)
                    {
                        IEnumerator bss = networkExplorer.broadcastsReceived.Values.GetEnumerator();
                        bss.MoveNext();
                        lobbyController.networkAddress = ((NetworkBroadcastResult)bss.Current).serverAddress;
                        yield return new WaitForSeconds(0.1f);

                        SafeStopBroadcast();

                        try
                        {
                            lobbyController.StartClient();
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(e.Message);
                            continue;
                        }

                        startdAsClient = true;
                    }
                    yield return new WaitForSeconds(waitTime);
                }

                if (!startdAsClient)
                {
                    SafeStopBroadcast();
                    yield return new WaitForSeconds(0.1f);

                    try
                    {
                        lobbyController.StartHost();
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e.Message);
                        continue;
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
                        
                        yield return new WaitForSeconds(waitTime* GetRandomAttempts());

                        if (NetworkServer.connections.Count < 2)
                        {
                            networkExplorer.StopBroadcast();
                            yield return new WaitForSeconds(0.1f);
                            startedAsServer = false;
                            lobbyController.SetFastStart(true);
                            yield return new WaitForSeconds(0.1f);
                            lobbyController.StopHost();
                        }
                    }
                }
            
            }
            while ( (!startdAsClient && !startedAsServer) && !restart );
            if(restart)
            Debug.LogWarning("Exiting with " + startdAsClient + " and " + startedAsServer);
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

        private int GetRandomAttempts()
        {
            return attempts + UnityEngine.Random.Range(-2, 2);
        }
    }
}
