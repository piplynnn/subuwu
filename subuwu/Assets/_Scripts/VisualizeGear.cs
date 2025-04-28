using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizeGear : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    void Update()
    {
        if (textComponent != null)
        {
            if (CarMath.gear == 0)
            {
                textComponent.text = "N";
            }
            else
            {
                textComponent.text = CarMath.gear.ToString();
                
            }
           
        }
    }
}