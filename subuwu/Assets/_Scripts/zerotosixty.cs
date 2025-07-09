using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class zerotosixty : MonoBehaviour
{
    public List<SpriteRenderer> lights = new List<SpriteRenderer>();
    private int lastIndex = 0;
    public TextMeshProUGUI timerText;

    private Color fullcol = new Color(1f, 1f, 1f, 1f); // Full alpha white
    private Color lightcol = new Color(1f, 1f, 1f, 80f / 255f); // Dim white
    private float timer;
    public bool isTiming = false;
    public bool red, yellow, green;
    public bool stopranonce = false;
    public bool started = false;

    private Coroutine lightRoutine;

    void Start()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            lights.Add(sr);
            Color newColor = sr.color;
            newColor.a = lightcol.a;
            sr.color = newColor;
        }
        lastIndex = lights.Count;
        timer = 0.0f;
    }

    void Update()
    {
  
        if (startPressed.startpressed && !started)
        {
            started = true;
            timer = 0f;
            timerText.text = "00.00";
            lightRoutine = StartCoroutine(StartSequence());
        }

        if (!startPressed.startpressed && started)
        {
            StopSequence();
        }
        if (isTiming)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("00.00");
        }
        if (green)
        {
            DataTimerCollect();
        }
    }

    private IEnumerator StartSequence()
    {
        
        red = true;
        ColorOn(0);
        yield return new WaitForSeconds(1.0f);

        
        yellow = true;
        ColorOn(1);
        yield return new WaitForSeconds(1.0f);

       
        green = true;
        ColorOn(2);
        
    }

    private void StopSequence()
    {
        if (lightRoutine != null)
        {
            StopCoroutine(lightRoutine);
        }

        started = false;
        red = yellow = green = false;
        stopranonce = true;
        isTiming = false;

        for (int i = 0; i < 3; i++)
            ColorOff(i);
        
    }

    private void ColorOn(int indexnum)
    {
        if (indexnum < lights.Count)
        {
            Color newColor = lights[indexnum].color;
            newColor.a = fullcol.a;
            lights[indexnum].color = newColor;
            lights[lastIndex - indexnum - 1].color = newColor;
        }
    }

    private void ColorOff(int indexnum)
    {
        if (indexnum < lights.Count)
        {
            Color newColor = lights[indexnum].color;
            newColor.a = lightcol.a;
            lights[indexnum].color = newColor;
            lights[lastIndex - indexnum - 1].color = newColor;
        }
    }

    private void DataTimerCollect()
    {
        if (CarData.mph <= 60)
        {
          
            isTiming = true;
        }
        else
        {
            startPressed.startpressed = false;
        }
        
    }
}
