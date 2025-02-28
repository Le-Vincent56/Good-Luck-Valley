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
            // Fade out the Canvas Group
            Fade(0f, fadeDuration, Ease.InOutSine, () =>
            {
                screen.interactable = false;
                screen.blocksRaycasts = false;

                Debug.Log("Faded out");
            });

            // Show the Journal
            EventBus<ShowJournalPause>.Raise(new ShowJournalPause());
        }

        public override void OnExit()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
