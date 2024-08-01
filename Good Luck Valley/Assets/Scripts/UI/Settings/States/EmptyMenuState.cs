using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Settings.States
{
    public class EmptyMenuState : SettingsState
    {
        public EmptyMenuState(GameSettingsMenu menu, StateMachine stateMachine, bool fadeInOut, Exclusions exclusions, GameObject uiObject, MenuCursor cursor) 
            : base(menu, stateMachine, fadeInOut, exclusions, uiObject, cursor)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}