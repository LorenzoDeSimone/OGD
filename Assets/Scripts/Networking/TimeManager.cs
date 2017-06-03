using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class TimeManager : NetworkBehaviour
{
    private float endTime;
    private Text myText;

    private void Start()
    {
        myText = GetComponentInChildren<Text>();
    }

    string minutes, seconds;
    int numMinutes, numSeconds;
    float remainingSeconds;

    private IEnumerator CountDown()
    {
        remainingSeconds = endTime - Time.time;
        if (remainingSeconds >= 0)
        {
            numSeconds = (int)(remainingSeconds % 60);
            numMinutes = (int)(remainingSeconds / 60);
            if (numSeconds < 10)
                seconds = "0" + numSeconds;
            else
                seconds = "" + numSeconds;
            if (numMinutes < 10)
                minutes = "0" + numMinutes;
            else
                minutes = "" + numMinutes;

            myText.text = minutes + ":" + seconds;
            yield return new WaitForSecondsRealtime(1);

            StartCoroutine(CountDown());
        }
    }

    [ClientRpc]
    public void RpcSetEndTime(float num)
    {
        endTime = Time.time + num;
        StartCoroutine(CountDown());
    }
}