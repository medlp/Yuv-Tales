using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsTabController : MonoBehaviour
{
    [System.Serializable]
    public class Tab
    {
        public Button tabButton;
        public GameObject tabPanel;
    }

    [SerializeField] private List<Tab> tabs;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.6f, 0.6f, 0.6f);

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

            var colors = tabs[i].tabButton.colors;
            colors.normalColor = isActive ? activeColor : inactiveColor;
            tabs[i].tabButton.colors = colors;
        }
    }
}