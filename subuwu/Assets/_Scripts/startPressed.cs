using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startPressed : MonoBehaviour
{
    public static bool startpressed = false;

    public void startpress()
    {
        startpressed = true;
    }

    private void Update()
    {
        
    }
}
