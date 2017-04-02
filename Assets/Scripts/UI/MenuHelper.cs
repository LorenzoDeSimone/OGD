using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.UI
{
    class MenuHelper : MonoBehaviour
    {
        public MenuHelper backButtonHelper;
        public List<GameObject> toActivate;
        public List<GameObject> toDeactivate;

        public void TriggerHelper()
        {
            SetList(false, toDeactivate);
            SetList(true, toActivate);

            backButtonHelper.toDeactivate = toActivate;
            backButtonHelper.toActivate = toDeactivate;
        }

        private void SetList(bool setVal, List<GameObject> list)
        {
            foreach (GameObject go in list)
            {
                go.SetActive(setVal);
            }
        }
    }
}
