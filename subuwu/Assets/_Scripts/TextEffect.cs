using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[System.Serializable]
public class TextEffect
{
    public GameObject textEffectObject;
    public TextMeshProUGUI dataText;
    public void MakeActive()
    {
        textEffectObject.SetActive(true);
    }

    public void MakeInactive()
    {
        textEffectObject.SetActive(false);
    }

}
