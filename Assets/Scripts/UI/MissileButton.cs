using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileButton : MonoBehaviour
{
    public static MissileButton instance;

    public void Activate()
    {
        GetComponent<Button>().interactable = true;
    }

    public void Deactivate()
    {
        GetComponent<Button>().interactable = false;
    }

    void Start()
    {
        if (instance == null)
            instance = this;
    }
}
