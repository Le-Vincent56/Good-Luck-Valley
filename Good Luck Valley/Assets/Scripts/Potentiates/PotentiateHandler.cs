using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Potentiates
{
    public class PotentiateHandler : MonoBehaviour
    {
        private PlayerController playerController;
        private Optional<Potentiate> lastPotentiate = Optional<Potentiate>.None();
        [SerializeField] private string lastPotentiateName;

        public PlayerController Controller { get => playerController; }

        private void Awake()
        {
            // Get components
            playerController = GetComponent<PlayerController>();
        }

        /// <summary>
        /// Set the last Potentiate collected
        /// </summary>
        public void SetLastPotentiate(Potentiate lastPotentiate)
        {
            this.lastPotentiate = lastPotentiate;
            lastPotentiateName = lastPotentiate.gameObject.name;
        }

        /// <summary>
        /// Deplete the last Potentiate
        /// </summary>
        public void DepletePotentiate()
        {
            // Deplete the last Potentiate
            lastPotentiate.Match(
                onValue: potentiate => { 
                    potentiate.Deplete();
                    RemovePotentiate();
                    return 0; 
                },
                onNoValue: () => { return 0; }
            );
        }

        /// <summary>
        /// Remove references to the last Potentiate
        /// </summary>
        public void RemovePotentiate()
        {
            lastPotentiate = null;
            lastPotentiateName = string.Empty;
        }
    }
}
