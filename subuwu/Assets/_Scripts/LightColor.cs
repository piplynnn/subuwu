using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColor : MonoBehaviour
{
    public List<GameObject> lights = new List<GameObject>(); 

   
    void Start()
    {
        foreach (Transform child in transform)
        {
           lights.Add(child.gameObject);
        }

        
    }

    
    void Update()
    {
        
    }
}
