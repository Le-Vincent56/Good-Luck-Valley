using GoodLuckValley.Patterns.Commands;
using UnityEngine;

namespace GoodLuckValley.World.Interactables
{
    public class Lantern : MonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField] private Transform fireflies;

        public bool HasFireflies => fireflies != null;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ExecuteCommand()
        {
            throw new System.NotImplementedException();
        }

        public void Interact()
        {
            throw new System.NotImplementedException();
        }

        public void QueueCommand(ICommand<IInteractable> command)
        {
            throw new System.NotImplementedException();
        }
    }
}