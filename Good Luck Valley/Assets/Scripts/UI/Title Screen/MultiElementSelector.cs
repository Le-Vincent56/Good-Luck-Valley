using GoodLuckValley.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Elements
{
    public class MultiElementSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Selectable selectable;
        [SerializeField] private List<Graphic> graphics;

        [SerializeField] private Color normalColor = new Color(255f, 255f, 255f, 0.46274509803f);
        [SerializeField] private Color highlightedColor = new Color(255f, 255f, 255f, 1f);
        [SerializeField] private Color selectedColor = new Color(255f, 255f, 255f, 1f);

        public void OnPointerEnter(PointerEventData eventData) => SetColor(highlightedColor);

        public void OnPointerExit(PointerEventData eventData)
        {
            if(EventSystem.current.currentSelectedGameObject != gameObject)
            {
                SetColor(normalColor);
            }
        }

        public void OnPointerClick(PointerEventData eventData) => SetColor(selectedColor);

        public void OnSelect(BaseEventData eventData) => SetColor(selectedColor);

        public void OnDeselect(BaseEventData eventData) => SetColor(normalColor);

        /// <summary>
        /// Set the color of the graphics
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(Color color)
        {
            foreach (Graphic g in graphics)
            {
                g.color = color;
            }
        }
    }
}