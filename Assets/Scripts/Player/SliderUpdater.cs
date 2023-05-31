using UnityEngine;
using UnityEngine.UI;

public class SliderUpdater : MonoBehaviour
{
    private Slider slider;
    void Start(){
        slider = this.GetComponent<Slider>();
    }
    public void UpdateSliderValue(int damage)
    {
        if (slider != null)
        {
            slider.value -= damage;
            Debug.Log(slider.gameObject.name + " " + slider.value);
        }
    }
}