using GoodLuckValley.Architecture.Optionals;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.OptionMenus
{
    public class SelectableOptionMenu : MonoBehaviour, IOptionMenu
    {
        [SerializeField] private Selectable initialOption;
        [SerializeField] private bool initialEveryTime;
        private Optional<Selectable> lastOption = Optional<Selectable>.None();

        public void SelectFirst()
        {
            // Check if a last option exists
            lastOption.Match(
                onValue: selectable =>
                {
                    // Check if the initial option should be selected every time
                    if(initialEveryTime)
                    {
                        // Select the initial option
                        initialOption.Select();
                    }
                    else
                    {
                        // Otherwise, select the Selectable
                        selectable.Select();
                    }

                    return 0;
                },
                onNoValue: () =>
                {
                    // Select the initial option
                    initialOption.Select();

                    return 0;
                }
            );
        }

        public void UpdateFirst()
        {
            // Set the last option as the currently selected Selectable
            lastOption = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
        }
    }
}
