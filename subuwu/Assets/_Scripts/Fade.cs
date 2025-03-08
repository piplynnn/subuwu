using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public static bool fadeIn = false;
    public static bool fadeOut = false;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.alpha = 0;
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
                }
            }
        }
    }

    private IEnumerator WaitAndTriggerFadeOut()
    {
        yield return new WaitForSeconds(2f);
        fadeOut = true;
    }
}