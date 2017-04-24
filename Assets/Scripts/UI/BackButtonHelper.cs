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
        NetworkLobbyController lobbyController;

        void OnEnable()
        {
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
        }

        public override void TriggerHelper()
        {
            if(resetLobbyController)
            {
              lobbyController.ResetAndStop();
              StartCoroutine(ResetLobbyWhenReady());
            }
            else
            {
                SetListsStates();
            }
        }

        private IEnumerator ResetLobbyWhenReady()
        {
            yield return new WaitUntil(lobbyController.IsReadyToReset);
            SetListsStates();
        }

        private void SetListsStates()
        {
            SetList(false, toDeactivate);
            SetList(true, toActivate);
        }
    }
}
