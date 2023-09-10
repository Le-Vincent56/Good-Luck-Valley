using UnityEngine;
using UnityEngine.SceneManagement;
using HiveMind.Core;

namespace HiveMind.Interactables
{
    public class AnguishLotus : Interactable
    {
        public override void Interact()
        {
            // Interact with the lotus
            Debug.Log("Lotus Picked");
            finishedInteracting = true;

            // Load new scene
            SceneManager.LoadScene("Title Screen");
        }
    }
}