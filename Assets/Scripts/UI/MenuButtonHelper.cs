using UnityEngine;

namespace Assets.Scripts.UI
{
    class MenuButtonHelper : MenuHelper
    {
        public BackButtonHelper backButtonHelper;
        public bool backButtonWillResetLobbyController = true;

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
