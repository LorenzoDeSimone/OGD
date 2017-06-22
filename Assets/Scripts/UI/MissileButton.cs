using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileButton : MonoBehaviour
{
    public static MissileButton instance;
    private Button button;
    private Image image;

    public void Activate()
    {
        button.interactable = true;
        image.color = button.colors.normalColor;
    }

    public void Deactivate()
    {
        button.interactable = false;
        image.color = button.colors.disabledColor;
    }

    void Start()
    {
        if (instance == null)
            instance = this;
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }
}
