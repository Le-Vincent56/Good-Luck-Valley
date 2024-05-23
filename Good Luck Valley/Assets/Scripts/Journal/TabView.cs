using GoodLuckValley.Journal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour
{
    [SerializeField] public JournalTab[] tabs;

    private void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].Initialize(i, i == 0);
        }
    }

    public void SelectTab(int tab)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].selected = (i == tab) ? true : false;
        }
    }
}
