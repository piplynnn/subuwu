using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject SettingsMenu;
    public List<GameObject> ScreenButtons;

    private bool lastSettingsState;

    void Start()
    {
        lastSettingsState = SettingsMenu.activeInHierarchy;
        UpdateButtons();
    }

    void Update()
    {
        
        if (lastSettingsState != SettingsMenu.activeInHierarchy)
        {
            lastSettingsState = SettingsMenu.activeInHierarchy;
            UpdateButtons();
        }
    }

    void UpdateButtons()
    {
        bool showButtons = !SettingsMenu.activeInHierarchy;

        for (int i = 0; i < ScreenButtons.Count; i++)
        {
            ScreenButtons[i].SetActive(showButtons);
        }
    }
}