public class ToggleOnline : SliderHelper
{
    protected override void Init()
    {
    }

    protected override void OnValueChanged(float newValue)
    {
        if((int)newValue == 0)
        {
            lobbyController.Online = true; 
        }
        else
        {
            lobbyController.Online = false;
        }
    }
}
