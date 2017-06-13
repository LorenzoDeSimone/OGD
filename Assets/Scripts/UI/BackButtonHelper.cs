using System;
using Assets.Scripts.Networking;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    class BackButtonHelper : MenuHelper
    {
        internal bool resetLobbyController = true;

        public override void TriggerHelper()
        {
            if(resetLobbyController)
            {
              lobbyController.PrepareToReset();
              StartCoroutine(ResetLobbyWhenReady());
            }
            else
            {
                SetListsStates();
            }
        }

        internal override void Init()
        {
        }

        private IEnumerator ResetLobbyWhenReady()
        {
            yield return new WaitUntil(lobbyController.IsReadyToReset);
            lobbyController.ResetNetworkState();
            SetListsStates();
        }

        private void SetListsStates()
        {
            SetList(false, toDeactivate);
            SetList(true, toActivate);
        }
    }
}
