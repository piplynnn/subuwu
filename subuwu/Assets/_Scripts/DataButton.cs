using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataButton : MonoBehaviour
{
    public GameObject DataCanv;
    public GameObject startbutton;
    public static bool stop = false;
    public void ButtonClicked()
    {
        DataCanv.SetActive(false);
        startbutton.SetActive(true);
        startPressed.startpressed = false;
        stop = true;
        
        

    }
}
