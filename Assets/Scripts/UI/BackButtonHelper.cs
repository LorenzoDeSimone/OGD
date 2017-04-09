using System;
using Assets.Scripts.Networking;
using UnityEngine;
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
            }

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
