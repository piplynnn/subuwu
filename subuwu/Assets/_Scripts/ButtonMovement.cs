using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMovement : MonoBehaviour
{
    public static bool leftPressed = false;
   public static bool rightPressed = false;

    public void leftButtonPressed()
    {
        if (CamMovement.screennum != -1) leftPressed = true;
        
    }

    public void rightButtonPressed()
    {
        if (CamMovement.screennum != 1) rightPressed = true;
        
    }

}
