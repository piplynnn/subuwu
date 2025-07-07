using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startPressed : MonoBehaviour
{
    public static bool startpressed = false;

    public void startpress()
    {
        if (!startpressed)
        {
            startpressed = true;
        }
        else
        {
            startpressed = false;
        }
    }

    private void Update()
    {
        
    }
}
