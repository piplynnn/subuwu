using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public GameObject cam;

    void Update()
    {
        if (FadeIN.fadeonce && !FadeIN.fadeIn)
        {
            if(Input.GetKey(KeyCode.RightArrow))
            {
                Vector3 newPos = new Vector3(transform.position.x + 1280, transform.position.y, transform.position.z);
                StartCoroutine(LerpTra)
            }
        }
        
    }
}
