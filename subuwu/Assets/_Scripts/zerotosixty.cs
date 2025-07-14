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
    public TextMeshProUGUI startText;

    private Color fullcol = new Color(1f, 1f, 1f, 1f); // Full alpha white
    private Color lightcol = new Color(1f, 1f, 1f, 80f / 255f); // Dim white

    private const int RED = 0;
    private const int YELLOW = 1;
    private const int GREEN = 2;
    private float timer;
    public float totalRPM;
    public bool isTiming = false;
    public bool red, yellow, green;
    public bool stopranonce = false;
    public bool started = false;
    public static bool is_data_colecting;
    public static float zerotosixtyRPMtotal = 0;
    public static float zerotosixtyRPMcount = 1;
    
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
        //add if car gets early start, !Startpressed.Startpressed
        
        if (startPressed.startpressed && !started)
        {
            started = true;
            timer = 0f;
            timerText.text = "00.00";
            startText.text = "Stop";
            lightRoutine = StartCoroutine(StartSequence());
        }

        if (!startPressed.startpressed && started )
        {
            StopSequence();
            startText.text = "Start";
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
        yield return new WaitForSeconds(1.5f);
        ColorOn(RED);
        yield return new WaitForSeconds(1.0f);
        ColorOn(YELLOW);
        yield return new WaitForSeconds(1.0f);
        green = true;
        ColorOn(GREEN);
        
    }

    private void StopSequence()
    //stop sequence isnt used to stop data, but rather reset everything back to normal.
    {
        StopCoroutine(lightRoutine);
        started = false;
        is_data_colecting = false;
        isTiming = false;
        green = false;
        stopranonce = true;
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
        //add the time between go and takeoff
        if (CarData.mph < 60)
        {
            isTiming = true;
            is_data_colecting = true;
            totalRPM = zerotosixtyRPMtotal / zerotosixtyRPMcount;
        }
        else 
        {
            is_data_colecting = false;
            isTiming = false;
            DisplayData();
        }
        
    }

    private void DisplayData()
    {
        
    }
}
