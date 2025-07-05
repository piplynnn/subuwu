using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class zerotosixty : MonoBehaviour
{

    public List<SpriteRenderer> lights = new List<SpriteRenderer>();
    private int lastIndex = 0;
    private Color fullcol;

    private Color lightcol;
    public bool start = false;
    public bool red;
    public bool yellow;
    public bool green;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            lights.Add(sr);
            Color newColor = sr.color;
            fullcol.a = 1f;
            newColor.a = 80f / 255f;
            //lightcol.a = 20/255f;
            sr.color = newColor;
            lastIndex = lights.Count;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if (startPressed.startpressed)
        {
           start = true;
           if (start)
           {
               StartSequence();
           }

        }
    }

    private void StartSequence()
    {
        StartCoroutine(TimerCoroutine(() => red = true));
        if (red)
        {
            StartCoroutine(TimerCoroutine(() => yellow = true));
        }

        if (yellow)
        {
            StartCoroutine(TimerCoroutine(() => green = true));
        }
    }

    IEnumerator TimerCoroutine(Action light)
    {
        yield return new WaitForSeconds(1.0f);
        light();
    }
}

