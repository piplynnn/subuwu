using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataMoveItem
{
    public Vector2 position;
    public GameObject textObject;
    [HideInInspector] public bool hasMoved = false;

    public void InitPosition()
    {
        position = textObject.GetComponent<RectTransform>().anchoredPosition;
    }
    public void ResetPosition()
    {
        if (textObject != null)
        {
            textObject.GetComponent<RectTransform>().anchoredPosition = position;
            textObject.SetActive(false);
            hasMoved = false;
        }
    }
}
