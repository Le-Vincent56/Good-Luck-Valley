using UnityEngine;

namespace HiveMind.Tutorial
{
    public class HideTutorialMessage : MonoBehaviour
    {

        #region REFERENCES
        [SerializeField] private TutorialMessage tutorialMessage;
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                tutorialMessage.RemoveMessage = true;
            }
        }
    }
}
