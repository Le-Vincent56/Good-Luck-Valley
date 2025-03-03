using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley
{
    public class PointerTest : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log($"Clicked on: {gameObject}");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log($"Hovered over: {gameObject}");
        }
    }
}
