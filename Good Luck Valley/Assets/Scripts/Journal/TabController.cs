using GoodLuckValley.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public class TabController
    {
        readonly TabView view;
        readonly TabModel model;
        readonly Queue<TabCommand> tabQueue = new Queue<TabCommand>();
        readonly CountdownTimer timer = new CountdownTimer(0);

        public TabController(TabView view, TabModel model)
        {
            this.view = view;
            this.model = model;
        }
    }
}