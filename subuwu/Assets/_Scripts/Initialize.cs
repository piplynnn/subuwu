using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.IO.Ports;
using System.Threading;

using DG.Tweening;

public class Initialize : MonoBehaviour
{
    private GameObject initslash;
    private GameObject obdslash;
    private GameObject ecuslash;
    private GameObject sdslash;

    public GameObject canv;

    public GameObject slashes;
    public GameObject sti;


    public Sprite check;
    public Sprite uncheck;

    public SpriteRenderer stisprite;

    public bool initoutcome = false;
    public bool obdoutcome = false;
    public bool obdchecked = false;
    public bool obdranonce = false;
    public bool ecuoutcome = false;
    public bool ecuchecked = false;
    public bool ecuranonce = false;
    public bool sdoutcome = false;
    public bool sdchecked = false;
    public bool sdranonce = false;
    private bool isRunning = true;
    private bool check1 = false;
    private bool check2 = false;
    private bool check3 = false;

    private float waitTime1;
    private float waitTime2;
    private float waitTime3;
    private string latestRPM = "0";
    public SerialPort serialPort = new SerialPort("COM3", 115200);
    private Thread obdThread;



    private float interval = 1f / 12f;
    private float timer = 0f;

    public float fadeDuration = 1f;



    void Start()
    {
        try
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                Debug.Log("Serial Port Opened!");
            }
            if (obdThread == null || !obdThread.IsAlive) // ✅ Ensure the thread starts only once
            {
                obdThread = new Thread(ReadOBDData);
                obdThread.Start();
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError("Error opening serial port: " + e.Message);
        }
        
        
        initslash = GameObject.Find("Init-S");
        obdslash = GameObject.Find("OBD-S");
        ecuslash = GameObject.Find("ECU-S");
        sdslash = GameObject.Find("SD-S");
        waitTime1 = Random.Range(1f, 2f);
        waitTime2 = Random.Range(3f, 5f);
        waitTime3 = Random.Range(5f, 6f);
        stisprite = sti.GetComponent<SpriteRenderer>();
     
    }

    void Update()
    {
        timer += Time.deltaTime;
        initi();
        obd();
        ecu();
        sd();
        Debug.Log (latestRPM);
       

        if (timer >= interval)
        {
            if (!initoutcome && initslash != null)
                initslash.transform.Rotate(0, 0, -30f);

            if (!obdoutcome && obdslash != null)
                obdslash.transform.Rotate(0, 0, -30f);

            if (!ecuoutcome && ecuslash != null)
                ecuslash.transform.Rotate(0, 0, -30f);

            if (!sdoutcome && sdslash != null)
                sdslash.transform.Rotate(0, 0, -30f);

            timer = 0f;
        }
    }

    public void initi()
    {

        if (check1 && check2 && check3)
        {
            initslash.GetComponent<SpriteRenderer>().sprite = check;
            initslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            initslash.transform.localScale = new Vector3(1, 1, 1);
            initoutcome = true;
            StartCoroutine(Initdone());

        }

    }

    public void obd()
    {
        obdchecked = true;
        //try
        //catch


        if (obdchecked)
        {
            StartCoroutine(ActivateOBD());

        }


    }

    public void ecu()
    {
        ecuchecked = true;
        if (ecuchecked)
        {
            StartCoroutine(ActivateECU());

        }

    }


    public void sd()
    {
        sdchecked = true;
        if (sdchecked)
        {
            StartCoroutine(ActivateSD());
        }

    }

    IEnumerator ActivateOBD()
    {
        yield return new WaitForSeconds(waitTime1);
        obdoutcome = true;
        if (obdchecked && !obdranonce)
        {
            obdslash.GetComponent<SpriteRenderer>().sprite = check;
            obdslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            obdslash.transform.localScale = new Vector3(1, 1, 1);
            Debug.Log("Bool set to TRUE after " + waitTime1 + " seconds.");
            obdranonce = true;
            check1 = true;

        }

    }

    IEnumerator ActivateECU()
    {

        yield return new WaitForSeconds(waitTime2);
        ecuoutcome = true;
        if (ecuchecked && !ecuranonce)
        {
            ecuslash.GetComponent<SpriteRenderer>().sprite = check;
            ecuslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            ecuslash.transform.localScale = new Vector3(1, 1, 1);
            Debug.Log("Bool set to TRUE after " + waitTime2 + " seconds.");
            ecuranonce = true;
            check2 = true;
        }

    }

    IEnumerator ActivateSD()
    {
        yield return new WaitForSeconds(waitTime3);
        sdoutcome = true;
        if (sdchecked && !sdranonce)
        {
            sdslash.GetComponent<SpriteRenderer>().sprite = check;
            sdslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            sdslash.transform.localScale = new Vector3(1, 1, 1);
            Debug.Log("Bool set to TRUE after " + waitTime3 + " seconds.");
            sdranonce = true;
            check3 = true;
        }
    }

    IEnumerator Initdone()
    {
        yield return new WaitForSeconds(2f);
        canv.SetActive(false);
        slashes.SetActive(false);

        // Wait for FadeIn to complete
        yield return FadeIn();

        yield return new WaitForSeconds(2f);

        // Wait for FadeOut to complete
        yield return FadeOut();
    }

    public IEnumerator FadeIn()
    {
        yield return stisprite.DOColor(new Color(stisprite.color.r, stisprite.color.g, stisprite.color.b, 1f), fadeDuration).WaitForCompletion();
    }

    public IEnumerator FadeOut()
    {
        yield return stisprite.DOColor(new Color(stisprite.color.r, stisprite.color.g, stisprite.color.b, 0f), fadeDuration).WaitForCompletion();
    } 
    void ReadOBDData()
    {
        Debug.Log("OBD-II Thread Started!"); // ✅ Confirm thread starts

        while (isRunning && serialPort.IsOpen)
        {
            try
            {
                serialPort.WriteLine("01 0C\r"); // Request RPM
                string response = serialPort.ReadLine().Trim(); // Remove extra spaces/newlines
                Debug.Log("Raw OBD Response: [" + response + "]"); // ✅ Debugging raw response

                latestRPM = ParseRPM(response);
            }
            catch (System.TimeoutException)
            {
                latestRPM = "No Data";
            }
            catch (System.Exception e)
            {
                Debug.LogError("Serial Read Error: " + e.Message);
            }
            Thread.Sleep(100);
        }
    }


string ParseRPM(string rawResponse)
{
    if (string.IsNullOrEmpty(rawResponse))
    {
        return "Invalid Data";
    }

    // ✅ Extract the last two hex values (XX YY)
    string[] parts = rawResponse.Split(' ');
    if (parts.Length >= 4 && parts[0] == "41" && parts[1] == "0C") // Ensure correct response
    {
        int rpm = (int.Parse(parts[2], System.Globalization.NumberStyles.HexNumber) * 256) +
                  int.Parse(parts[3], System.Globalization.NumberStyles.HexNumber);
        return (rpm / 4).ToString(); // Divide by 4 for actual RPM
    }

    return "Invalid Data"; // If the format is incorrect
}

    void OnDestroy()
    {
        isRunning = false;
        if (obdThread != null) obdThread.Join();
        if (serialPort.IsOpen) serialPort.Close();
    }
}
