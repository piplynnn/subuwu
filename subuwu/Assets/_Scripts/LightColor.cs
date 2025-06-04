using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColor : MonoBehaviour
{
    public List<SpriteRenderer> lights = new List<SpriteRenderer>();
    
    public bool green = false;
    public bool yellow1 = false;
    public bool yellow2 = false;
    public bool red = false;
    private int lastIndex = 0;
    private Color fullcol;
    private Color lightcol;
    

   
    void Start()
    {
        foreach (Transform child in transform)
        { 
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
           lights.Add(sr);
           Color newColor = sr.color;
           fullcol.a = 1f;
           newColor.a = 20f/255f;
           lightcol.a = 20/255f;
           sr.color = newColor;
           lastIndex = lights.Count;

        }
        
    }

    
    void Update()
    {
        ColorHandling();
        if (CarData.rpm > 4250)
        {
            green = true;
        }
        else
        {
            green = false;
        }
        if (CarData.rpm > 4750)
        {
            yellow1 = true;
           
        }
        else
        {
            yellow1 = false;
        }
        if (CarData.rpm > 5250)
        {
            yellow2 = true;
           
        }
        else
        {
            yellow2 = false;
        }
        if (CarData.rpm > 6000)
        {
            red = true;
           
        }
        else
        {
            red = false;
        }
        
    }

    private void ColorHandling()
    {
        if (green)
        {
            ColorOn(0);

        }
        else
        {
            ColorOff(0);
           
            
        }
        if (yellow1)
        {
            ColorOn(1);

        }
        else
        {
            ColorOff(1);
           
            
        }
        if (yellow2)
        {
            ColorOn(2);
       
        }
        else
        {
            ColorOff(2);
        }
        if (red)
        {
            ColorOn(3);
        }
        else
        {
            ColorOff(3);
        }
        
    }

    private void ColorOn(int indexnum)
    {
        Color newColor = lights[indexnum].color; 
        newColor.a = fullcol.a;
        lights[indexnum].color = newColor;
        lights[lastIndex-indexnum-1].color = newColor;
    }

    private void ColorOff(int indexnum)
    {
        Color newColor = lights[indexnum].color; 
        newColor.a = lightcol.a;
        lights[indexnum].color = newColor;
        lights[lastIndex-indexnum -1].color = newColor;
    }
    
    
}
