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
    public static int gear;
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
        float[] gearRatios = { 3.80f, 2.10f, 1.45f, 1.00f, 0.80f, 0.65f };
        int bestGear = 0;
        if (mph == 0 || (rpm <= 1000 && mph > 5))
        {
            gear = 0;
            return;
        }
        float bestDifference = float.MaxValue;
        for (int i = 0; i < gearRatios.Length; i++)
        {
            float difference = Mathf.Abs(gearRatio - gearRatios[i]);
            if (difference < bestDifference)
            {
                bestDifference = difference;
                bestGear = i + 1; 
            }
        }

        gear = bestGear; 
    }
    
    }         

