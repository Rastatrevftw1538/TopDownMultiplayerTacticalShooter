using UnityEngine;
using UnityEngine.UI;

public class SliderSyncSP : MonoBehaviour
{
    private Slider[] sliderArray;

    private void Awake()
    {
        sliderArray = GetComponentsInChildren<Slider>();
    }

    public void Start()
    {
        foreach (var slider in sliderArray)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        CmdSyncSliderValue(value);
    }

    private void CmdSyncSliderValue(float value)
    {
        // Update the server-side value with the new slider value
        // (You can implement your own logic here based on your requirements)
        // For example:
        // health = value;
    }
}
