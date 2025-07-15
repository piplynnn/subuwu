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
    public GameObject zerotosixText;
    public GameObject avgRPMtext;
    public GameObject maxRPMtext;
    public GameObject launchDelayText;
    public GameObject dataDisplay;
    public GameObject startButton;
    private RectTransform displayrect;
    private Vector2 velocity = new Vector2(300f, 0f); // px/sec
    private Vector2 currentPosition;
    private Vector2 endpos = new Vector2(4f, -122f);

    private Color fullcol = new Color(1f, 1f, 1f, 1f); // Full alpha white
    private Color lightcol = new Color(1f, 1f, 1f, 80f / 255f); // Dim white

    private const int RED = 0;
    private const int YELLOW = 1;
    private const int GREEN = 2;
    private static readonly Vector2 TextMovePos = new Vector2(-370f, 60f);
    public float displaySpeed = 2f;
    private float timer;
    public float textmoveSpeed = 20000;
    public float avgRPM;
    public bool isTiming = false;
    public bool red, yellow, green;
    public bool stopranonce = false;
    public bool started = false;
    public bool isDisplayingData = false;
    public bool testbool = false;
    public bool displaySlideBool = false;
    public static bool is_data_colecting;
    public static float zerotosixtyRPMtotal;
    public static float zerotosixtyRPMcount;
    private bool isSliding = false;
    public bool slideDone = false;
    private float slideDuration = 1f; // seconds
    private float slideTimer = 0f;

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
        displayrect = dataDisplay.GetComponent<RectTransform>();
        currentPosition = displayrect.anchoredPosition;
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
            zerotosixtyRPMtotal = 0;
            zerotosixtyRPMcount = 1;
            lightRoutine = StartCoroutine(StartSequence());
        }

        if (!startPressed.startpressed && started)
        {
            StopSequence();
            DisplayData(); //remove when done testing display data
            startText.text = "Start";
        }

        if (isTiming)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("00.00");
        }

        if (dataDisplay.activeInHierarchy)
        {

            if (isSliding)
            {
                slideTimer += Time.deltaTime;
                float t = Mathf.Clamp01(slideTimer / slideDuration);
                t = Mathf.SmoothStep(0f, 1f, t);
                displayrect.anchoredPosition = Lerpers.PositionLerp2d(currentPosition, endpos, t);
                if (t >= 1f)
                {
                    isSliding = false;
                    slideDone = true;
                }

            }


        }

        if (slideDone)
        {
            StartCoroutine(DataMove());
        }



        if (green)
        {
            DataTimerCollect();
        }
    }

    private IEnumerator StartSequence()
    {
        yield return new WaitForSeconds(1.5f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.raceStart);
        ColorOn(RED);
        

        yield return new WaitForSeconds(1.0f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.raceStart);
        ColorOn(YELLOW);
        
        
        yield return new WaitForSeconds(1.0f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.raceGo);
        green = true;
        ColorOn(GREEN);

    }

    private void StopSequence()
        //stop sequence isnt used to stop data, but rather reset everything back to normal such as timer and lights.
    {
        StopCoroutine(lightRoutine);
        started = false;
        is_data_colecting = false;
        displaySlideBool = false;
        isTiming = false;
        isSliding = false;
        slideDone = false;
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
        if (CarData.mph < 60 && !testbool)
        {
            isTiming = true;
            is_data_colecting = true;
            avgRPM = zerotosixtyRPMtotal / zerotosixtyRPMcount;
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

        startButton.SetActive(false);
        dataDisplay.SetActive(true);
        if (!displaySlideBool)
        {
            isSliding = true;
            displaySlideBool = true;
        }


        

    }
    public void RectMoverX(GameObject text, RectTransform rect, Vector2 newpos, float speed)
    {
        Vector2 movetoPos = new Vector2(newpos.x, rect.anchoredPosition.y);
        Vector2 currentPos = rect.anchoredPosition;
        float step = speed * Time.deltaTime;
        float distance = Vector2.Distance(currentPos, movetoPos);
        if (distance <= step)
        {
            rect.anchoredPosition = movetoPos; 
            text.SetActive(true);
            return;
        }
        Vector2 direction = (movetoPos - currentPos).normalized;
        rect.anchoredPosition = currentPos + direction * step;
        text.SetActive(true);
    }


    private IEnumerator DataMove()
    {
        RectMoverX(zerotosixText, zerotosixText.GetComponent<RectTransform>(),TextMovePos, textmoveSpeed);
        yield return new WaitForSeconds(0.2f);
        RectMoverX(avgRPMtext, avgRPMtext.GetComponent<RectTransform>(),TextMovePos, textmoveSpeed);
        yield return new WaitForSeconds(0.2f);
        RectMoverX(maxRPMtext, maxRPMtext.GetComponent<RectTransform>(),TextMovePos, textmoveSpeed);
        yield return new WaitForSeconds(0.2f);
        RectMoverX(launchDelayText, launchDelayText.GetComponent<RectTransform>(),TextMovePos, textmoveSpeed);
        
    }

}