using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CountDown : NetworkBehaviour
{
    Text myText;
    SpriteRenderer mySprite;

    private void Start()
    {
        myText = GetComponentInChildren<Text>();
        mySprite = GetComponentInChildren<SpriteRenderer>();
        mySprite.enabled = true;
    }
    
    [ClientRpc]
    public void RpcChangeNetworkState(int count)
    {
        mySprite.enabled = true;
        StopCoroutine("countdown");
        StartCoroutine(countdown(count));
    }

    private IEnumerator countdown(int count)
    {
        while (count > 0)
        {
            myText.text = count + "";
            yield return new WaitForSecondsRealtime(1);
            count--;
        }
        myText.text = "";
        //mySprite.enabled = false;
    }
}
