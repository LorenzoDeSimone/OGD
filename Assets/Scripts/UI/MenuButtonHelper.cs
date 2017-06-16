using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    class MenuButtonHelper : MenuHelper
    {
        public BackButtonHelper backButtonHelper;
        public bool backButtonWillResetLobbyController = true;
        public bool enableFastRestart = false;

        internal override void Init()
        {
            //nothing to init here
        }

        public override void TriggerHelper()
        {
            if (backButtonHelper)
            {
                backButtonHelper.toDeactivate = toActivate;
                backButtonHelper.toActivate = toDeactivate;
                backButtonHelper.resetLobbyController = backButtonWillResetLobbyController; 
            }
            
            SetList(false, toDeactivate);
            SetList(true, toActivate);
        }
    }
}
