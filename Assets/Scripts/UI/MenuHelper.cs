using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Assets.Scripts.Networking;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(Button))]
    abstract class MenuHelper : MonoBehaviour
    {
        public List<GameObject> toActivate;
        public List<GameObject> toDeactivate;

        private Button attachedButton;

        void Start()
        {
            attachedButton = GetComponent<Button>();
            attachedButton.onClick.AddListener(TriggerHelper);
        }

        public abstract void TriggerHelper();

        protected void SetList(bool setVal, List<GameObject> list)
        {
            foreach (GameObject go in list)
            {
                go.SetActive(setVal);
            }
        }
    }
}
