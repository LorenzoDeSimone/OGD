using System;
using Assets.Scripts.Networking;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.UI
{
    class BackButtonHelper : MenuHelper
    {
        public string lobbyControllerTag = "NetworkLobbyController";
        internal bool resetLobbyController = true; 

        public override void TriggerHelper()
        {
            if(resetLobbyController)
            {
              GetLobbyController().ResetAndStop();
              StartCoroutine(ResetLobbyWhenReady());
            }
            else
            {
                SetListsStates();
            }

        }

        private IEnumerator ResetLobbyWhenReady()
        {
            Debug.LogError(GetLobbyController().IsReadyToReset());
            yield return new WaitUntil(GetLobbyController().IsReadyToReset);
            Debug.LogError(GetLobbyController().IsReadyToReset());
            SetListsStates();
        }

        private void SetListsStates()
        {
            SetList(false, toDeactivate);
            SetList(true, toActivate);
        }

        private NetworkLobbyController GetLobbyController()
        {
            GameObject go = GameObject.FindGameObjectWithTag(lobbyControllerTag);
            return go.GetComponent<NetworkLobbyController>();
        }

    }
}
