using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    class PublicPlayMenu : PlayMenu
    {
        protected override void InitMenu()
        {
            TryJoinMatch();
            StartCoroutine(SpinLoadingSpinner());
        }

        protected override void TryJoinMatch()
        {
            lobbyController.StartMatchMaker();

            lobbyController.readyToReset = false;
            StartCoroutine(SearchMatch());
        }

        private IEnumerator SearchMatch()
        {
            while (lobbyController.IsSearchingPublicMatch())
            {
                lobbyController.loadingMatches = true;
                
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
                        if (mis.currentSize > 0)
                        {
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
        protected override void TryInitMenu()
        {
            if (lobbyController.Online)
            {
                lobbyController.currentPlayMenu = this;
                InitMenu();
            }
        }
    }
}
