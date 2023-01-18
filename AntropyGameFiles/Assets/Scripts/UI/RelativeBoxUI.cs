using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RelativeBoxUI : MonoBehaviour
{

    public RectTransform box;
    public RectTransform indicator;

    public void SetLeftRight(float middle, float delta, float maxVal, float overshooting){
        float width = box.rect.width;
        Debug.Log("Width of the outer box: " + width);
        float leftBorder; 
        float rightBorder;
        if (delta > 0)
        {
            leftBorder  = middle / maxVal * width;
            rightBorder = -(maxVal - Mathf.Min(middle + delta, maxVal * (1+overshooting))) / maxVal * width; //right overshooting limit 1.17
        }
        else
        {
            leftBorder = Mathf.Max((middle + delta)/ maxVal * width, -width * overshooting); // left undershooting limit -0.17
            rightBorder = -(maxVal - middle) / maxVal * width;
        }
        Debug.Log("Left: " + leftBorder + ", right: " + rightBorder);
        indicator.offsetMin = new Vector2(leftBorder,0);
        indicator.offsetMax = new Vector2(rightBorder,0);


    }
}
