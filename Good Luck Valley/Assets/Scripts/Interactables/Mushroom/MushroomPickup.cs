using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using GoodLuckValley.Interactables.Mushroom.States;
using GoodLuckValley.Timers;
using UnityEngine;
using UnityEngine.Playables;

namespace GoodLuckValley.Interactables.Mushroom
{
    public class MushroomPickup : Collectible
    {
        [SerializeField] private int hash;
        [SerializeField] private float effectTime;
        [SerializeField] private ParticleSystem ambientParticles;
        [SerializeField] private ParticleSystem pickParticles;
        private StateMachine stateMachine;
        private CountdownTimer effectTimer;
        private bool triggerEffect;
        
        
        public int Hash => hash;
        public bool Active { get; set; }
        
        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();
            
            // Create the effect timer
            CreateEffectTimer();
            
            // Set up the state machine
            SetupStateMachine();

            Active = true;
            
            // Set the strategy
            strategy = new MushroomPickupStrategy(this, fadeDuration, GetComponent<PlayableDirector>());
        }

        private void Update()
        {
            // Exit case - not active
            if (!Active) return;
            
            // Update the state machine
            stateMachine?.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Dispose of the timer
            DisposeOfTimer();
        }
        
        /// <summary>
        /// Interact with the Collectible
        /// </summary>
        public override void Interact()
        {
            // Exit case - the Handler has no value
            if (!handler.HasValue) return;

            // Exit case - the Interactable cannot be interaacted with
            if (!canInteract) return;

            // Exit case - the Interactable Strategy fails
            if (!strategy.Interact(handler.Value)) return;

            // Remove the Interactable from the Handler
            handler.Match(
                onValue: handler =>
                {
                    handler.SetInteractable(Optional<Interactable>.NoValue);
                    return 0;
                },
                onNoValue: () => { return 0; }
            );

            // Set un-interactable
            canInteract = false;

            // Set collected
            collected = true;
            data.Collected = collected;

            // Check if any other collection logic is needed
            Collect();
        }

        /// <summary>
        /// Hide the mushroom
        /// </summary>
        public void HideShroom()
        {
            FadeInteractable(0f, fadeDuration);
            EventBus<EndCinematic>.Raise(new EndCinematic());
        }

        private void CreateEffectTimer()
        {
            triggerEffect = false;
            
            // Create the timer
            effectTimer = new CountdownTimer(effectTime);
            
            // Hook up events
            effectTimer.OnTimerStop += SetTrigger;

            // Start the effect timer
            effectTimer.Start();
        }

        private void DisposeOfTimer()
        {
            effectTimer.OnTimerStop -= SetTrigger;
            effectTimer.Dispose();
        }

        private void SetTrigger() => triggerEffect = true;

        public void ResetTrigger()
        {
            triggerEffect = false;
            effectTimer.Start();
        }
        
        public void PlayParticles() => pickParticles?.Play();

        private void SetupStateMachine()
        {
            // Initialize the state machine
            stateMachine = new StateMachine();
            
            Animator animator = GetComponent<Animator>();
            
            // Create states
            MushroomIdleState idle = new MushroomIdleState(this, animator, ambientParticles);
            MushroomParticleState effect = new MushroomParticleState(this, animator, ambientParticles);
            
            // Define state transitions
            stateMachine.At(idle, effect, new FuncPredicate(() => triggerEffect));
            stateMachine.At(effect, idle, new FuncPredicate(() => !triggerEffect));
            
            // Set initial state
            stateMachine.SetState(idle);
        }
    }
}
