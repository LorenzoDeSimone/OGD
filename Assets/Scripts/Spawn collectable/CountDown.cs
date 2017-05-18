using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CountDown : NetworkBehaviour
{
    Text myText;

    private void Start()
    {
        myText = GetComponentInChildren<Text>();
    }
    
    [ClientRpc]
    public void RpcChangeNetworkState(string str)
    {
        myText.text = str;
    }
}
