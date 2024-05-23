using GoodLuckValley.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public class TabModel
    {
        public readonly ObservableList<Tab> tabs = new ObservableList<Tab>();

        public void AddTab(Tab tab) => tabs.Add(tab);
    }

    public class Tab
    {
        public readonly TabData data;

        public Tab(TabData data)
        {
            this.data = data;
        }

        public TabCommand CreateCommand()
        {
            return new TabCommand(data);
        }
    }
}
