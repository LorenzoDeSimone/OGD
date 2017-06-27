using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.UI;

public class OfflineActivator : MonoBehaviour
{
    void Update ()
    {
        GetComponent<Text>().enabled = !NetworkLobbyController.instance.Online;
	}
}
