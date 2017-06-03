using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    class LanPlayMenu : PlayMenu
    {
        NetworkDiscovery networkExplorer;

        public float waitTime = 0.3f;
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

            networkExplorer.Initialize();
            yield return new WaitForSeconds(waitTime);
            networkExplorer.StartAsClient();

            for (int i = 0; i < attempts; i++)
            {
                if (networkExplorer == null || networkExplorer.broadcastsReceived == null)
                {
                    i--;
                    continue;
                }

                if (networkExplorer.broadcastsReceived.Count > 0)
                {
                    IEnumerator bss = networkExplorer.broadcastsReceived.Values.GetEnumerator();
                    bss.MoveNext();
                    lobbyController.networkAddress = ((NetworkBroadcastResult)bss.Current).serverAddress;
                    lobbyController.StartClient();
                    startdAsClient = true;
                }
                yield return new WaitForSeconds(waitTime);
            }

            if (!startdAsClient)
            {
                networkExplorer.StopBroadcast();
                yield return new WaitForSeconds(waitTime); try
                {
                    lobbyController.StartHost();
                }
                catch (Exception)
                {
                    StartCoroutine(SearchMatch());
                }
                finally
                {
                    networkExplorer.Initialize();
                    networkExplorer.StartAsServer();
                }
            }
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
