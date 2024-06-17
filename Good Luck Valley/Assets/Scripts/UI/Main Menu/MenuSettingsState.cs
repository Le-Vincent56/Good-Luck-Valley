using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu
{
    public class MenuSettingsState : MenuState
    {
        public MenuSettingsState(MenuController menu, MenuStateMachine stateMachine, bool fadeInOut, GameObject uiObject)
            : base(menu, stateMachine, fadeInOut, uiObject)
        {
        }
    }
}
