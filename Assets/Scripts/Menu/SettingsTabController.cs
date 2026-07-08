using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsTabController : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button tabButton;
        public GameObject tabPanel;
    }

    [SerializeField] private List<Tab> tabs;
    [SerializeField] private SettingsManager settingsManager;

    private void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i; 
            tabs[i].tabButton.onClick.AddListener(() => SelectTab(index));
        }

        SelectTab(0); 
    }

    public void SelectTab(int index)
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            bool isActive = (i == index);
            tabs[i].tabPanel.SetActive(isActive);
        }

        if (settingsManager != null)
            settingsManager.SetCurrentTab(index);

        EventSystem.current.SetSelectedGameObject(null);
    }
}