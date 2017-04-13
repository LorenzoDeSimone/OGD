using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    class PublicPlayMenu : MonoBehaviour
    {
        public MaskableGraphic loadingSpinner;
        public MaskableGraphic loadingMessage;
        public string lobbyControllerTag = "NetworkLobbyController";

        void OnEnable()
        {
            GetLobbyController().JoinPublicMatch();
            StartCoroutine(SpinLoadingSpinner());
        }

        private IEnumerator SpinLoadingSpinner()
        {
            while(true)
            {
                loadingSpinner.transform.Rotate(loadingSpinner.transform.forward,2.0f);
                yield return new WaitForEndOfFrame();
            }
        }

        protected NetworkLobbyController GetLobbyController()
        {
            GameObject go = GameObject.FindGameObjectWithTag(lobbyControllerTag);
            return go.GetComponent<NetworkLobbyController>();
        }
    }
}
