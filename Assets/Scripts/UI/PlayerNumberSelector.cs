using Assets.Scripts.Networking;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerNumberSelector : SliderHelper {

    public Text textField;

    protected override void Init(float newValue)
    {
        lobbyController.SetMinPlayers((int)newValue);
    }

    protected override void OnValueChanged(float newValue)
    {
        textField.text = "" + (int)newValue;
        lobbyController.SetMinPlayers((int)newValue);
    }
}
