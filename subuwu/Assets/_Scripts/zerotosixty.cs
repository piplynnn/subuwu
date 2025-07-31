using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.Remoting.Messaging;
using Unity.VisualScripting.Dependencies.NCalc;

public class zerotosixty : MonoBehaviour
{
    
    public List<SpriteRenderer> lights = new List<SpriteRenderer>();
    
    private int lastIndex = 0;
    
    [Header("TMPro textures")]
    
    public TextMeshProUGUI timerText;
    
    public TextMeshProUGUI startText;
    
    
    
    [Header("Display Text Objects")]
    public List<DataMoveItem> dataItems = new List<DataMoveItem>();
    public List<TextEffect> dataTextobjects = new List<TextEffect>();
    
    public GameObject zerotosixText;
    
    public GameObject avgRPMtext;
    
    public GameObject maxRPMtext;
    
    public GameObject launchDelayText;

    public GameObject zerotosixData;
    
    public GameObject avgRPMData;
    
    public GameObject maxRPMData;
    
    public GameObject launchDelayData;
    
    
    public GameObject dataDisplay;
    
    public GameObject startButton;
    
    private RectTransform displayrect;
    
    [Header("Colors")]
    
    [SerializeField]
    
    private Color fullcol = new Color(1f, 1f, 1f, 1f); // Full alpha white
    
    [SerializeField]
    
    private Color lightcol = new Color(1f, 1f, 1f, 80f / 255f); // Dim white

    private const int RED = 0;
    
    private const int RED2 = 1;
    
    private const int RED3 = 2;
    [Header("Text Locations/Speed")]
    
    [SerializeField]
    
    private Vector2 DataDisplayBeforePos;
    
    [SerializeField]
    
    private Vector2 DataDisplayEndPos = new Vector2(4f, -122f);
    
    [SerializeField]
    
    private Vector2 TextMovePos = new Vector2(-450f, 60f);
    
    [SerializeField]
    
    private float textmoveSpeed = 20000;
    [SerializeField]
    private float timer;

    [SerializeField] private float LDelayTimer;

    [Header("Stat Numbers")] 
    private const int ZeroToSixtyInArray = 0;
    private const int AvgRPMInArray = 1;
    private const int MaxRPMInArray= 2;
    private const int LDelayInArray = 3;
    
    [SerializeField]
    
    private float avgRPM;
    
    [SerializeField]
    
    private float maxRPM;
    
    [Header("GameState bools")]
    
    public bool isTiming = false;
    
    [SerializeField]
    
    private bool sixtysound = false
    
    [SerializeField]
    
    private bool displayingDataRanOnce;

    [SerializeField] private bool DataMoveRanOnce;
    
    [SerializeField]
    
    private bool sixtyhit = false;

    [SerializeField] private bool CanShowData = false;
    
    [SerializeField]
    
    private bool green;
    
    [SerializeField]
    
    private bool started = false;
    
    public bool testbool = false;
    
    private bool LDelayTiming = false;
    private bool LDelayTimingRanOnce = false;
    private bool ShowDataRanOnce = false;

    
    
    [SerializeField] private bool displaySlideBool = false;
    
    public static bool is_data_colecting;
    
    public static float zerotosixtyRPMtotal;
    
    public static float zerotosixtyRPMcount;
    
    [Header("Slide Settings")]
    
    [SerializeField]
    
    private bool isSliding = false;
    
    [SerializeField]
    
    private bool slideDone = false;
    
    [SerializeField]
    
    private float slideDuration = 1f; 
    
    [SerializeField]
    
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
        foreach (DataMoveItem item in dataItems)
        {
            item.InitPosition();
        }
        

        lastIndex = lights.Count;
        timer = 0.0f;
        LDelayTimer = 0.0f;
        displayrect = dataDisplay.GetComponent<RectTransform>();
        DataDisplayBeforePos = displayrect.anchoredPosition;
    }
    

    void Update()
    {
        //add if car gets early start, !Startpressed.Startpressed

        if (startPressed.startpressed && !started)
        {
            started = true;
            timer = 0f;
            LDelayTimer = 0.0f;
            timerText.text = "00.00";
            dataTextobjects[LDelayInArray].dataText.text = "00.00";
            startText.text = "Stop";
            zerotosixtyRPMtotal = 0;
            zerotosixtyRPMcount = 1;
            lightRoutine = StartCoroutine(StartSequence());
        }

        if (!startPressed.startpressed && started)
        {
            StopSequence();
            startText.text = "Start";
        }

        if (isTiming)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("00.00");
        }

        if (LDelayTiming)
        {
            LDelayTimer += Time.deltaTime;
            dataTextobjects[LDelayInArray].dataText.text = LDelayTimer.ToString("00.00");
        }

        if (dataDisplay.activeInHierarchy)
        {

            if (isSliding)
            {
                slideTimer += Time.deltaTime;
                float t = Mathf.Clamp01(slideTimer / slideDuration);
                t = Mathf.SmoothStep(0f, 1f, t);
                displayrect.anchoredPosition = Lerpers.PositionLerp2d(DataDisplayBeforePos, DataDisplayEndPos, t);
                if (t >= 1f)
                {
                    isSliding = false;
                    slideDone = true;
                }

            }


        }

        if (slideDone && !DataMoveRanOnce)
        {
            StartCoroutine(DataMove());
            
        }

        if (CanShowData)
        {
            StartCoroutine(ShowData());
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
        ColorOn(RED2);
        
        
        yield return new WaitForSeconds(1.0f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.raceStart);
        ColorOn(RED3);
        yield return new WaitForSeconds(1.0f);
        AllColorChange(lights, Color.green);
        SoundManager.Instance.PlaySound(SoundManager.Instance.raceGo);
        green = true;


    }

    public void StopSequence()
        //stop sequence isnt used to stop data, but rather reset everything back to normal such as timer and lights.
    {



        StopCoroutine(lightRoutine);
        started = false;
        is_data_colecting = false;
        displaySlideBool = false;
        sixtyhit = false;
        isTiming = false;
        isSliding = false;
        slideDone = false;
        sixtysound = false;
        green = false;
        displayingDataRanOnce = false;
        startButton.SetActive(true);
        dataDisplay.SetActive(false);
        slideTimer = 0;
        AllColorChange(lights, Color.red);
        for (int i = 0; i < 3; i++)
            ColorOff(i);

        foreach (DataMoveItem item in dataItems)
        {
          item.ResetPosition();
        }
        foreach (var item in dataTextobjects)
        {
            item.MakeInactive();
        }
        displayrect.anchoredPosition = DataDisplayBeforePos;

        DataMoveRanOnce = false;
        CanShowData = false;
        LDelayTimingRanOnce = false;
        ShowDataRanOnce = false;

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

    private void AllColorChange(List<SpriteRenderer> _lights, Color color)
    {
        foreach (SpriteRenderer _light in _lights)
        {
            SpriteRenderer sr = _light.GetComponent<SpriteRenderer>();
            sr.color = color;
        }

        lastIndex = lights.Count;
        
    }

    private void DataTimerCollect()
    {
        //add the time between go and takeoff
        int maxRPM = 0;
        if (CarData.mph < 60 && !testbool && !sixtyhit)
        {
            isTiming = true;
            is_data_colecting = true;
            avgRPM = zerotosixtyRPMtotal / zerotosixtyRPMcount;
            if (!LDelayTimingRanOnce)
            {
                LDelayTiming = true;
                LDelayTimingRanOnce = true;
            }
            if (CarData.rpm > maxRPM)
            {
                maxRPM = CarData.rpm;
            }

            if (CarData.mph > 0)
            {
                LDelayTiming = false;
            }
            
        }
        else
        {
            AllColorChange(lights, Color.red);
            sixtyhit = true;
            is_data_colecting = false;
            dataTextobjects[ZeroToSixtyInArray].dataText.text = timer.ToString("00.00");
            dataTextobjects[AvgRPMInArray].dataText.text = avgRPM.ToString();
            dataTextobjects[MaxRPMInArray].dataText.text = maxRPM.ToString();
            isTiming = false;
            LDelayTiming = false;
            if (!sixtysound)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.sixty);
                sixtysound = true;
            }

            if (!displayingDataRanOnce)
            {
                StartCoroutine(DisplayTimer());
            }
            
            
        }

    }

    private void DisplayData()
    {
        if (!displayingDataRanOnce)
        {
            startButton.SetActive(false);
            dataDisplay.SetActive(true);
            displayingDataRanOnce = true;
        }
        
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
        float swooshLength = SoundManager.Instance.swoosh.length;

        foreach (var item in dataItems)
        {
            RectTransform rect = item.textObject.GetComponent<RectTransform>();
            RectMoverX(item.textObject, rect, TextMovePos, textmoveSpeed);

            if (!item.hasMoved)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.swoosh);
                item.hasMoved = true;
            }

            yield return new WaitForSeconds(swooshLength);
        }

        DataMoveRanOnce = true;
        CanShowData = true;

    }

    private IEnumerator ShowData()
    {
        if (!ShowDataRanOnce)
        {
            foreach (var item in dataTextobjects)
            {
                item.MakeActive();
                yield return new WaitForSeconds(0.2f);
            }
            ShowDataRanOnce = true;
        }

    }


    private IEnumerator DisplayTimer()
    {
        yield return new WaitForSeconds(2f);
        DisplayData();
        
    }

}