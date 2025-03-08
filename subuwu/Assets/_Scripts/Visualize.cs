using TMPro;
using UnityEngine;

public class Visualize : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    void Update()
    {
        if (textComponent != null)
        {
            textComponent.text = CarData.mph.ToString() + " mph";
        }
    }
}