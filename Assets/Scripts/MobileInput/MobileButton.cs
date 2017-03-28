using UnityEngine;

namespace Assets.Scripts.MobileInput
{
    class MobileButton : MonoBehaviour
    {
        void OnMouseDown()
        {
            Debug.Log(gameObject.name+" "+gameObject.GetInstanceID()+" is down");
        }
    }
}
