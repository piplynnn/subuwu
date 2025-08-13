using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataReset : MonoBehaviour
{
    public void ResetAllStats()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
