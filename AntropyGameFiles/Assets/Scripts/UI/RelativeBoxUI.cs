using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RelativeBoxUI : MonoBehaviour
{

    public RectTransform box;
    public RectTransform indicator;

    public void SetLeftRight(float middle, float delta, float maxVal){
        float width = box.rect.width;
        Debug.Log("Width of the outer box: " + width);
        float leftBorder; 
        float rightBorder;
        if (delta > 0)
        {
            leftBorder  = middle / maxVal * width;
            rightBorder = -(maxVal - (middle + delta )) / maxVal * width;
        }
        else
        {
            leftBorder = (middle + delta)/ maxVal * width;
            rightBorder = -(maxVal - middle) / maxVal * width;
        }
        Debug.Log("Left: " + leftBorder + ", right: " + rightBorder);
        indicator.offsetMin = new Vector2(leftBorder,0);
        indicator.offsetMax = new Vector2(rightBorder,0);


    }
}
