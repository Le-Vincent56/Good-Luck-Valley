using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Journal;
using GoodLuckValley.UI.Menus.OptionMenus;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.Menus.Pause.States
{
    public class JournalPauseState : PauseMenuState
    {
        public JournalPauseState(PauseMenuController controller, CanvasGroup screen, IOptionMenu optionMenu, float fadeDuration) 
            : base(controller, screen, optionMenu, fadeDuration)
        {
        }

        public override void OnEnter()
        {
            controller.HideBackground(() =>
            {
                screen.interactable = false;
                screen.blocksRaycasts = false;
            });

            // Show the Journal
            EventBus<ShowJournalPause>.Raise(new ShowJournalPause());

            Debug.Log("Entered Journal State");
        }

        public override void OnExit()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
