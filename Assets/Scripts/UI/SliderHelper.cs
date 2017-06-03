using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public abstract class SliderHelper : MonoBehaviour
{
    protected Slider mySlider;
    protected NetworkLobbyController lobbyController;

    void Start()
    {
        mySlider = GetComponent<Slider>();
        mySlider.onValueChanged.AddListener(OnValueChanged);
        lobbyController = (NetworkLobbyController)NetworkManager.singleton;
        Init();
    }

    protected abstract void Init();
    protected abstract void OnValueChanged(float newValue);
}
