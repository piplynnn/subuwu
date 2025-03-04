using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Initialize : MonoBehaviour
{
    private GameObject initslash;
    private GameObject obdslash;
    private GameObject ecuslash;
    private GameObject sdslash;

    public Sprite check;
    public Sprite uncheck;

    public bool initoutcome = false;
    public bool obdoutcome = false;
    public bool ecuoutcome = false;
    public bool sdoutcome = false;
    public bool check1 = false;


    private float interval = 1f / 12f; // Every 1/3 of a second
    private float timer = 0f;

    void Start()
    {
        initslash = GameObject.Find("Init-S");
        obdslash = GameObject.Find("OBD-S");
        ecuslash = GameObject.Find("ECU-S");
        sdslash = GameObject.Find("SD-S");
    }

    void Update()
    {
        timer += Time.deltaTime;
        initi();


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

        if (check1)
        {
            initslash.GetComponent<SpriteRenderer>().sprite = check;
            initslash.transform.rotation = Quaternion.Euler(0, 0, 0);
            initslash.transform.localScale = new Vector3(1, 1, 1);

        }

    }

    public void obd()
    {
        check1 = true;


    }
}