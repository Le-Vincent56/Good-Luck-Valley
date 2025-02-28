using GoodLuckValley.UI.Menus.Controls;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley
{
    public class TutorialKeybind : MonoBehaviour
    {
        [SerializeField] private InputBindingInfo bindingInfo;
        private Animator animator;
        private Image image;

        public InputBindingInfo BindingInfo { get => bindingInfo; }

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            image = GetComponent<Image>();
        }

        /// <summary>
        /// Set the Tutorial Keybind's Animator
        /// </summary>
        public void SetAnimatorController(RuntimeAnimatorController animationController) => animator.runtimeAnimatorController = animationController;

        /// <summary>
        /// Set the Tutorial Keybind's Image
        /// </summary>
        public void SetSprite(Sprite sprite) => image.sprite = sprite;
    }
}
