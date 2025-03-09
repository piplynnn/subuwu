using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public static bool fadeIn = false;
    public static bool fadeOut = false;
    public static bool cammove = false;
    public GameObject firstscreen;
    [SerializeField] private CanvasGroup canvasGroup;
    public Camera mainCamera;
    public Camera secondCamera;

    private void Awake()
    {
        secondCamera.enabled = false;
        canvasGroup.alpha = 0;
        firstscreen = GameObject.Find("Canvas0");
        
    }

    private void Update()
    {
        if (fadeIn)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.deltaTime;
                if (canvasGroup.alpha >= 1)
                {
                    canvasGroup.alpha = 1;
                    fadeIn = false;
                    // Once fully faded in, wait 3 seconds then trigger fade out.
                    StartCoroutine(WaitAndTriggerFadeOut());
                }
            }
        }

        if (fadeOut)
        {
            if (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime;
                if (canvasGroup.alpha <= 0)
                {
                    canvasGroup.alpha = 0;
                    fadeOut = false;
                    cammove = true;
                    StartCoroutine(FadeInCanv0());
                }
            }
        }
    }
    void SwitchCamera()
    {
        mainCamera.enabled = false;  
        secondCamera.enabled = true; 
    }

    private IEnumerator WaitAndTriggerFadeOut()
    {
        yield return new WaitForSeconds(2f);
        fadeOut = true;
    }

    private IEnumerator FadeInCanv0()
    {
        yield return new WaitForSeconds(1.5f);
        SwitchCamera();
        FadeIN.fadeIn = true;
        
        
    }
 
}