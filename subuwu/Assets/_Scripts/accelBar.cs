using UnityEngine;
using UnityEngine.UI;

public class AccelerationBar : MonoBehaviour
{
    public Image accelBarFill;
    public float maxSpeed = 100f; // tweak based on your game's top speed
    public float carspeed = 20f;

    void Update()
    {
        
        float normalizedSpeed = Mathf.Clamp01(carspeed / maxSpeed);

        // Update fill amount
        accelBarFill.fillAmount = normalizedSpeed;

        // Update color (green to red)
        accelBarFill.color = Color.Lerp(Color.green, Color.red, normalizedSpeed);
    }
}