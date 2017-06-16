using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Networking;

namespace Assets.Scripts.Networking
{
    public class ServerModeSetter : MonoBehaviour
    {
        Toggle toggle;
        public LanPlayMenu lanPlayMenu;
        public Image toggleImage;

        void Start()
        {
            toggle = GetComponent<Toggle>();

#if UNITY_STANDALONE_WIN
            toggle.isOn = true;
#endif
            UpdateServerMode();
        }

        public void UpdateServerMode()
        {
            lanPlayMenu.isHostingMatch = toggle.isOn;
            toggleImage.enabled = toggle.isOn;
        }
    } 
}
