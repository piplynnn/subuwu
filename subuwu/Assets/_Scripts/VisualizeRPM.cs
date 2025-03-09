using TMPro;
using UnityEngine;

public class VisualizeRPM : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    void Update()
    {
        if (textComponent != null)
        {
            textComponent.text = CarData.rpm.ToString();
        }
    }
}