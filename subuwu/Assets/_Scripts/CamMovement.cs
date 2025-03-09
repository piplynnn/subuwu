using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public GameObject cam;
    public static int screennum = 0;

    void Update()
    {
        if (FadeIN.fadeonce && !FadeIN.fadeIn)
        {
            float camX = Mathf.Abs(cam.transform.position.x);

            if (Input.GetKeyDown(KeyCode.RightArrow) && (camX - 640) % 1280 == 0 && screennum < 1)
            {
                Vector3 newPos = new Vector3(cam.transform.position.x + 1280, cam.transform.position.y, cam.transform.position.z);
                StartCoroutine(Lerpers.LerpTransform(cam.transform, newPos, Lerpers.OutQuad(0.1f)));
                screennum++;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) && (camX - 640) % 1280 == 0 && screennum > -1)
            {
                Vector3 newPos = new Vector3(cam.transform.position.x - 1280, cam.transform.position.y, cam.transform.position.z);
                StartCoroutine(Lerpers.LerpTransform(cam.transform, newPos, Lerpers.OutQuad(0.1f)));
                screennum--;
            }
        }
    }

}