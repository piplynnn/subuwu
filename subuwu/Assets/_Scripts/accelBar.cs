using UnityEngine;
using UnityEngine.UI;

public class AccelerationBar : MonoBehaviour
{
    public Image accelBarFill;
    public float maxaccel = 100f; 
    public float normalizedAccel;
    public float currentaccel = 0f; // change with throttle when hooked up to car
    

    void Update()
    {
        normalizedAccel = ((maxaccel - CarData.throttleper) / maxaccel);
        accelBarFill.fillAmount = normalizedAccel;
        accelBarFill.color = Color.black;
    }
}