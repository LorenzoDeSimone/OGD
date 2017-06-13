using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class TimeManager : NetworkBehaviour
{
    [SyncVar]
    private double endTime;
    private Text myText;

    private void Start()
    {
        myText = GetComponentInChildren<Text>();
        StartCoroutine(CountDown());
    }

    string minutes, seconds;
    int numSeconds;
    double remaningTime;

    private IEnumerator CountDown()
    {
        remaningTime = endTime - Network.time;
        if (remaningTime >= 0)
        {
            numSeconds = (int)(remaningTime % 60);
            minutes = "" + (int)(remaningTime / 60);
            if (numSeconds < 10)
                seconds = "0" + numSeconds;
            else
                seconds = "" + numSeconds;

            myText.text = minutes + ":" + seconds;
        }
            yield return new WaitForSecondsRealtime(1);
            StartCoroutine(CountDown());
    }
    
    public void SetEndTime(double num)
    {
        endTime = num;
    }
}