using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuState
    {
        #region REFERENCES
        protected MenuController menu;
        protected MenuStateMachine stateMachine;
        #endregion

        #region PROPERTIES
        public bool FadeInOut { get; private set; }
        #endregion

        public MenuState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut)
        {
            this.menu = menu;
            this.stateMachine = stateMachine;
            FadeInOut = fadeInOut;
        }

        /// <summary>
        /// Enter a Menu State
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Exit a Menu State
        /// </summary>
        public virtual void Exit() { }

        /// <summary>
        /// Update Menu State logic
        /// </summary>
        public virtual void LogicUpdate() { }
    }
}