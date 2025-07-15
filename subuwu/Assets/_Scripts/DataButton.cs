using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataButton : MonoBehaviour
{
    public GameObject DataCanv;
    public GameObject startbutton;
    public void ButtonClicked()
    {
        startPressed.startpressed = false;
        DataCanv.SetActive(false);
        startbutton.SetActive(true);

    }
}
