using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class SliderSync : NetworkBehaviour
{
    private Slider[] sliderArray;

    private void Awake()
    {
        sliderArray = GetComponentsInChildren<Slider>();
    }

    public override void OnStartAuthority()
    {
        foreach (var slider in sliderArray)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    public override void OnStopAuthority()
    {
        foreach (var slider in sliderArray)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        CmdSyncSliderValue(value);
    }

    [Command]
    private void CmdSyncSliderValue(float value)
    {
        // Update the server-side value with the new slider value
        // (You can implement your own logic here based on your requirements)
        // For example:
        // health = value;
    }
}
