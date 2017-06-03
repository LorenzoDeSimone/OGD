using Assets.Scripts.Networking;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerNumberSelector : SliderHelper {

    public Text textField;

    protected override void Init()
    {
    }

    protected override void OnValueChanged(float newValue)
    {
        textField.text = "" + (int)newValue;
        lobbyController.minPlayers = (int)newValue;
    }
}
