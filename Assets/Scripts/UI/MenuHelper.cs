using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.Scripts.Networking;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    abstract class MenuHelper : MonoBehaviour
    {
        public List<GameObject> toActivate;
        public List<GameObject> toDeactivate;
        protected NetworkLobbyController lobbyController;

        private Button attachedButton;

        void Start()
        {
            attachedButton = GetComponent<Button>();
            attachedButton.onClick.AddListener(TriggerHelper);
            lobbyController = (NetworkLobbyController)NetworkManager.singleton;
            Init();
        }

        public abstract void TriggerHelper();

        protected void SetList(bool setVal, List<GameObject> list)
        {
            foreach (GameObject go in list)
            {
                go.SetActive(setVal);
            }
        }

        void OnEnable()
        {
            if(lobbyController == null)
            {
                lobbyController = (NetworkLobbyController)NetworkManager.singleton;
                if(lobbyController)
                {
                    Init();
                }
            }
        }

        internal abstract void Init();
    }
}
