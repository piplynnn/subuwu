using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;


public class CamMovement : MonoBehaviour
{
    public GameObject cam;
    public static int screennum = 0;
    static public int prevscreennum = 0;
    public static bool moved = true;

    void Update()
    {
        if (FadeIN.fadeonce && !FadeIN.fadeIn)
        {
            float camX = Mathf.Abs(cam.transform.position.x);

            if ((Input.GetKeyDown(KeyCode.RightArrow) || ButtonMovement.rightPressed) && (camX - 640) % 1280 == 0 && screennum < 1)
            {
                Vector3 newPos = new Vector3(cam.transform.position.x + 1280, cam.transform.position.y, cam.transform.position.z);
                StartCoroutine(Lerpers.LerpTransform(cam.transform, newPos, Lerpers.OutQuad(0.1f)));
                prevscreennum = screennum;
                screennum++;
                ButtonMovement.rightPressed = false;
                moved = true;
                
            }

            if ((Input.GetKeyDown(KeyCode.LeftArrow) || ButtonMovement.leftPressed) && (camX - 640) % 1280 == 0 && screennum > -1)
            {
                Vector3 newPos = new Vector3(cam.transform.position.x - 1280, cam.transform.position.y, cam.transform.position.z);
                StartCoroutine(Lerpers.LerpTransform(cam.transform, newPos, Lerpers.OutQuad(0.1f)));
                prevscreennum = screennum;
                screennum--;
                ButtonMovement.leftPressed = false;
                moved = true;
               
            }
        }
    }

}