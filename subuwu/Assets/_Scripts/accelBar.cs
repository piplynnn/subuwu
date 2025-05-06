using UnityEngine;
using UnityEngine.UI;

public class AccelerationBar : MonoBehaviour
{
    public Image accelBarFill;
    public float maxaccel = 100f; 
    public float normalizedAccel;
    public float currentaccel = 0f; // change with throttle when hooked up to car
    public int throttle = CarData.throttleper;

    void Update()
    {
        normalizedAccel = Mathf.Clamp01((maxaccel - currentaccel) / maxaccel);
        accelBarFill.fillAmount = normalizedAccel;
        accelBarFill.color = Color.black;
    }
}