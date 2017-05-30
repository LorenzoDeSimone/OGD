using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerNumberSelector : MonoBehaviour {

    public Text textField;
    protected NetworkLobbyController lobbyController;
    private Slider slider;
    
	void Start () {
        lobbyController = (NetworkLobbyController)NetworkManager.singleton;
        slider = GetComponent<Slider>();
    }
	
	public void ChangeValue()
    {
        textField.text = ""+(int)slider.value;
        lobbyController.minPlayers = (int)slider.value;
    }
}
