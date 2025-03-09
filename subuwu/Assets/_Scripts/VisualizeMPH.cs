using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizeMPH : MonoBehaviour
{
    public class VisualizeRPM : MonoBehaviour
    {
        public TextMeshProUGUI textComponent;

        void Update()
        {
            if (textComponent != null)
            {
                textComponent.text = CarData.mph.ToString();
            }
        }
    }
}