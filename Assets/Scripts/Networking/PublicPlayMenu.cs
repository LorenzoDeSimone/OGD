using System.Collections;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    class PublicPlayMenu : PlayMenu
    {
        const string CHARS_POOL = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789-";
        const int NAME_LENGHT = 16;

        public MaskableGraphic loadingSpinner;
        public MaskableGraphic loadingMessage;

        protected override void InitMenu()
        {
            JoinPublicMatch();
            StartCoroutine(SpinLoadingSpinner());
        }

        private void JoinPublicMatch()
        {
            lobbyController.StartMatchMaker();
            lobbyController.readyToReset = false;

            StartCoroutine(SearchMatch());
        }

        private IEnumerator SearchMatch()
        {
            while (lobbyController.IsSearchingPublicMatch())
            {
                lobbyController.loadingPublicMatches = true;
                lobbyController.matchMaker.ListMatches(0, 10, "", false, 0, 0, 
                    lobbyController.InitMatchList);
                yield return new WaitWhile(lobbyController.IsLoadingPublicMatches);

                if (lobbyController.publicMatches.Count == 0)
                {
                    lobbyController.creatingMatch = true;
                    lobbyController.CreateMatch(RandomPublicName());
                    yield return new WaitWhile(lobbyController.IsCreatingMatch);
                }
                else
                {
                    foreach (MatchInfoSnapshot mis in lobbyController.publicMatches)
                    {
                        string m = "";
                        m += mis.currentSize + "sz\n";
                        m += mis.hostNodeId + "node\n";
                        m += mis.networkId + "ner\n";
                        Debug.LogError(m);

                        if (mis.currentSize > 0)
                        {
                            m = "";
                            m += mis.currentSize + "sz\n";
                            m += mis.hostNodeId + "node\n";
                            m += mis.networkId + "ner\n";
                            Debug.LogError(m);

                            lobbyController.joiningMatch = true;
                            lobbyController.matchMaker.JoinMatch(mis.networkId, "", "", "", 0, 0,
                                lobbyController.OnMatchJoined);
                            yield return new WaitWhile(lobbyController.IsJoiningMatch);

                            if (!lobbyController.searchingPublicMatch)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator SpinLoadingSpinner()
        {
            while(true)
            {
                loadingSpinner.transform.Rotate(loadingSpinner.transform.forward,2.0f);
                yield return new WaitForEndOfFrame();
            }
        }

        private string RandomPublicName()
        {
            string randName = "";
            System.Random rInt = new System.Random();

            for (int i = 0; i < NAME_LENGHT; i++)
            {
                randName += CHARS_POOL[rInt.Next() % CHARS_POOL.Length];
            }

            return randName;
        }
    }
}
