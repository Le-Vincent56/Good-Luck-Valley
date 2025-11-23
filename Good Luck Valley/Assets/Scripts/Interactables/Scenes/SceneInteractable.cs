using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.Interactables.Scenes
{
    public class SceneInteractable : Interactable
    {
        [Header("Scene Settings")]
        [SerializeField] private SceneGroupData sceneGroupData;
        [SerializeField] private int sceneIndexToLoad;
        [SerializeField] private SceneGate toGate;
        [SerializeField] private int moveDirection = 0;
        [SerializeField] private bool showLoadingSymbol = true;
        
        private Optional<PlayerController> controller = Optional<PlayerController>.None();
        
        public SceneGroupData SceneGroupData => sceneGroupData;
        public int SceneIndexToLoad => sceneIndexToLoad;
        public SceneGate ToGate => toGate;
        public int MoveDirection => moveDirection;
        public bool ShowLoadingSymbol => showLoadingSymbol;
        public PlayerController Controller => controller.Value;
        
        
        protected override void Awake()
        {
            base.Awake();
            
            // Get the scene loader
            SceneLoader sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
            
            // Set the strategy
            strategy = new SceneInteractableStrategy(this, fadeDuration, sceneLoader);
        }
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // Exit case - the Interactable cannot be interacted with
            if (!canInteract) return;

            // Exit case - there's no Interactable Handler on the colliding object
            if (!other.TryGetComponent(out InteractableHandler handler) || !other.TryGetComponent(out PlayerController controller)) return;

            // Set the Interactable Handler
            this.handler = handler;
            
            // Set the Player Controller
            this.controller = controller;

            // Set this Interactable to be handled
            handler.SetInteractable(this);

            // Fade in the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 1f,
                Duration = fadeDuration
            });
        }

        protected override void OnTriggerStay2D(Collider2D other)
        {
            // Exit case - the trigger has already been activated
            if (triggered) return;

            // Exit case - the Interactable cannot be interacted with
            if (!canInteract) return;

            // Exit case - there's no Interactable Handler on the colliding object
            if (!other.TryGetComponent(out InteractableHandler handler) || !other.TryGetComponent(out PlayerController controller)) return;

            // Set the Interactable Handler
            this.handler = handler;
            
            // Set the Player Controller
            this.controller = controller;

            // Set this Interactable to be handled
            handler.SetInteractable(this);

            // Fade in the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 1f,
                Duration = fadeDuration
            });

            // Set triggered
            triggered = true;
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            // Exit case - the Interactable cannot be interacted with
            if (!canInteract) return;

            // Exit case - there's no Interactable Handler on the colliding object
            if (!other.TryGetComponent(out InteractableHandler handler) || !other.TryGetComponent(out PlayerController controller)) return;
            
            // Handle the Optional Interactable Handler
            this.handler.Match(
                // If there is a value, remove the Interactable from the Handler
                onValue: handler =>
                {
                    handler.SetInteractable(Optional<Interactable>.NoValue);
                    return 0;
                },
                onNoValue: () => { return 0; }
            );

            // Nullify the Interactable Handler
            this.handler = Optional<InteractableHandler>.NoValue;
            
            // Nullify the Controller
            this.controller = Optional<PlayerController>.NoValue;

            // Fade out the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup() 
            { 
                ID = id, 
                Value = 0f, 
                Duration = fadeDuration 
            });

            // Set not triggered
            triggered = false;
        }
    }

    public class SceneInteractableStrategy : InteractableStrategy
    {
        private readonly SceneInteractable parent;
        private readonly float fadeDuration;
        private readonly SceneLoader sceneLoader;

        public SceneInteractableStrategy(SceneInteractable parent, float fadeDuration, SceneLoader sceneLoader)
        {
            this.parent = parent;
            this.fadeDuration = fadeDuration;
            this.sceneLoader = sceneLoader;
        }
        
        public override bool Interact(InteractableHandler handler)
        {
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = parent.ID,
                Value = 0,
                Duration = fadeDuration,
            });
            
            // Load the scene
            // Exit case - already being forced to move
            if (parent.Controller.ForcedMove && parent.Controller.ForcedMoveDirection != 0) return false;
            
            // Exit case - already loading from gate
            if (sceneLoader.LoadingFromGate) return false;

            // Remove manual move
            parent.Controller.ForcedMove = true;
            parent.Controller.ForcedMoveDirection = parent.MoveDirection;

            // Set loading from gate
            sceneLoader.LoadingFromGate = true;

            // Start changing the scene group
            sceneLoader.ChangeSceneGroupLevel(parent.SceneIndexToLoad, parent.ToGate, parent.ShowLoadingSymbol, parent.MoveDirection);
            
            return true;
        }
    }
}
