using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColor : MonoBehaviour
{
    public List<SpriteRenderer> lights = new List<SpriteRenderer>();
    

   
    void Start()
    {
        foreach (Transform child in transform)
        {
           lights.Add(child.GetComponent<SpriteRenderer>());
        }

        
    }

    
    void Update()
    {
        if (CarData.rpm > 4500)
        {
            Color newColor = lights[0].GetComponent<SpriteRenderer>().color; 
            newColor.a = 44f / 100f;    
            sr.color = newColor;
        }
        
    }
}
