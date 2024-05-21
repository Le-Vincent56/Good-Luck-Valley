using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuInitialState : MenuState
    {
        public MenuInitialState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut) 
            : base(menu, stateMachine, fadeInOut)
        {
        }

        public override void Enter()
        {
            base.Enter();

            menu.Show(0);
        }

        public override void Exit()
        {
            base.Exit();

            menu.Hide(0);
        }

        public override void LogicUpdate()
        {
            if (menu.CheckStateChange() == 1) stateMachine.ChangeState(menu.MainState);
        }
    }
}