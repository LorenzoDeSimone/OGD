using Assets.Scripts.Networking;
using UnityEngine;

public class OnlineDeactivator : MonoBehaviour
{
    void Update ()
    {
	    if(NetworkLobbyController.instance.Online)
        {
            gameObject.SetActive(false);
        }
	}
}
