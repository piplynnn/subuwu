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
    public bool allred = false;
    private int lastIndex = 0;
    public List<Color> colors = new List<Color>();
    private Color fullred;
    private Color lightred;
    private Color fullcol;
    private Color lightcol;
    
    private Coroutine flashingRoutine;
    void Start()
    {
        lights.Clear();
        colors.Clear();

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
        
        lightred = new Color(1f, 0.5f, 0.5f, 1f);
        
        fullred = Color.red;
        for (int i = 0; i < 4; i++)
        {
            colors.Add(lights[i].color);
            
        }



    }

    
    void Update()
    {
     
        int rpm = CarData.rpm;

        green   = rpm > 4250;
        yellow1 = rpm > 4750;
        yellow2 = rpm > 5250;
        red     = rpm > 5750;
        allred   = rpm > 6000;
        

        ColorHandling();
        
    }


    

    private void ColorHandling()
    {
        if (!allred)
        {
            if (flashingRoutine != null)
            {
                StopCoroutine(flashingRoutine);
                flashingRoutine = null;
                for (int i = 0; i < 4; i++)
                {
                    NewColor(i, colors[i]);

                }
            }

            bool[] flags = { green, yellow1, yellow2, red };
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i]) ColorOn(i);
                else          ColorOff(i);
            }
        }
        else
        {
            if (flashingRoutine == null) // only start once
                flashingRoutine = StartCoroutine(ColorSwitch());
        }
    }

    private void ColorOn(int indexnum)
    {
        Color newColor = lights[indexnum].color; 
        newColor.a = fullcol.a;
        lights[indexnum].color = newColor;
        lights[lastIndex-indexnum-1].color = newColor;
    }
    private void NewColor(int indexnum, Color color)
    {
        Color newColor = color;
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
    private void AllColorChange(List<SpriteRenderer> _lights, Color color)
    {
        foreach (SpriteRenderer _light in _lights)
        {
            SpriteRenderer sr = _light.GetComponent<SpriteRenderer>();
            sr.color = color;
        }

        lastIndex = lights.Count;
        
    }

    private IEnumerator ColorSwitch()
    {
        while (allred) 
        {
            AllColorChange(lights, fullred);
            yield return new WaitForSeconds(0.1f);

            AllColorChange(lights, lightred);
            yield return new WaitForSeconds(0.1f);
        }
    }

    
    
}
