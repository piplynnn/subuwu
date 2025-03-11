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
    public AudioClip startup;
    public AudioSource audio;
    

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
    private bool check1 = false;
    private bool check2 = false;
    private bool check3 = false;
    private bool faderanonce = false;

    private float waitTime1;
    private float waitTime2;
    private float waitTime3;




    private float interval = 1f / 12f;
    private float timer = 0f;

    public float fadeDuration = 1f;




    void Start()
    {



        initslash = GameObject.Find("Init-S");
        obdslash = GameObject.Find("OBD-S");
        ecuslash = GameObject.Find("ECU-S");
        sdslash = GameObject.Find("SD-S");
        waitTime1 = Random.Range(1f, 2f);
        waitTime2 = Random.Range(3f, 5f);
        waitTime3 = Random.Range(5f, 6f);
        

    }

    void Update()
    {
        timer += Time.deltaTime;
        initi();
        obd();
        ecu();
        sd();



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

        if (obdranonce && !obdchecked || ecuranonce && !ecuchecked)
        {
            initslash.GetComponent<SpriteRenderer>().sprite = uncheck;
            initslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            initslash.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            initoutcome = true;

        }


    }

    public void obd()
    {
        obdchecked =true;
        StartCoroutine(ActivateOBD());
        /*
        if (CarData.ObdChecked && CarData.ObdFound)
        {
            obdchecked =true;
            StartCoroutine(ActivateOBD());
        }

        if (CarData.ObdChecked && !CarData.ObdFound)
        {
            obdchecked = false;
            StartCoroutine(ActivateOBD());

        }
        */
        

    }

    public void ecu()
    {
        
        ecuchecked = true;
        StartCoroutine(ActivateECU());
       /*
        if (CarData.EcuData && CarData.EcuCheck)
        {
            ecuchecked = true;
            StartCoroutine(ActivateECU());

        }

        if (!CarData.EcuData && CarData.EcuCheck)
        {
            ecuchecked = false;
            StartCoroutine(ActivateECU());
        }
       */
        

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

        if (!obdchecked && !obdranonce)
        {
            initslash.GetComponent<SpriteRenderer>().sprite = uncheck;
            obdslash.GetComponent<SpriteRenderer>().sprite = uncheck;
            obdslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            obdslash.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Debug.Log("Bool set to TRUE after " + waitTime1 + " seconds.");
            obdranonce = true;
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

        if (!ecuchecked && !ecuranonce)
        {
            ecuslash.GetComponent<SpriteRenderer>().sprite = uncheck;
            initslash.GetComponent<SpriteRenderer>().sprite = uncheck;
            ecuslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            ecuslash.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            Debug.Log("Bool set to TRUE after " + waitTime2 + " seconds.");
            ecuranonce = true;
            
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
        if (!faderanonce)
        {
            Fade.fadeIn = true;
            faderanonce = true;
            audio.PlayOneShot(startup);
        }
        
        
    }
    private IEnumerator AudioWait()
    {
        yield return new WaitForSeconds(2f);
        
    }
}

