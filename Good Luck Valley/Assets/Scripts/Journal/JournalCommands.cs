using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public interface ICommand
    {
        void Execute();
    }

    public class TabCommand : ICommand
    {
        private readonly TabData data;
        public float duration => data.Duration;

        public TabCommand(TabData data)
        {
            this.data = data;
        }

        public void Execute()
        {
            Debug.Log(data.ID);
        }
    }
}