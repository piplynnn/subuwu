using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMath : MonoBehaviour
{
    public static int totalrpm;
    public static int rpmAvg;
    public static int rpmcount;
    public static int totalmph;
    public static int rpmMph;
    public static int mphcount;
    public static float gearRatio;
    void Update()
    {
        if (CarData.BothActive)
        {
            
            Averages();
            GearCalc();

        }

    }

    public void Averages()
    {
        rpmAvg = totalrpm/rpmcount;
        rpmMph = totalmph/mphcount;
    }

    public void GearCalc()
    {
        int rpm = CarData.rpm;
        int mph = CarData.mph;
        float circumference = 80.7f;
        float wheelspeed = mph * 5280 * 12 / 60;
        float wheelrpm = wheelspeed / circumference;
        float finaldriveratio = 3.90f;
        gearRatio = (rpm / wheelrpm) / finaldriveratio;
    }         
}
