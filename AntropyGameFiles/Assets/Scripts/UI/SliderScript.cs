using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    public Slider slider;  

    public void SetMaxValue(int maxValue){
        slider.maxValue = maxValue;
    }
    public void SetValue(int value)
    {
        slider.value = value;
    }
}
