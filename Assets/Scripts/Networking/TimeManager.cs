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

    private IEnumerator CountDown()
    {
        if (endTime >= 0)
        {
            numSeconds = (int)(endTime % 60);
            minutes = "" + (int)(endTime / 60);
            if (numSeconds < 10)
                seconds = "0" + numSeconds;
            else
                seconds = "" + numSeconds;

            myText.text = minutes + ":" + seconds;
            endTime--;
        }
        yield return new WaitForSecondsRealtime(1);
        StartCoroutine(CountDown());
    }

    public void SetEndTime(double num)
    {
        endTime = num;
    }
}