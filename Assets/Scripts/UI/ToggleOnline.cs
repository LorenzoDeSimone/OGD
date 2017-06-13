public class ToggleOnline : SliderHelper
{
    protected override void Init(float newValue)
    {
        UpdateState(newValue);
    }

    protected override void OnValueChanged(float newValue)
    {
        UpdateState(newValue);
    }

    private void UpdateState(float newValue)
    {
        if ((int)newValue == 0)
        {
            lobbyController.Online = true;
        }
        else
        {
            lobbyController.Online = false;
        }
    }
}
