using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley.Player.Mushroom
{
    public class MushroomState : IState
    {
        protected readonly MushroomObject mushroomObject;
        protected readonly Animator animator;

        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int BounceHash = Animator.StringToHash("Bounce");
        protected static readonly int GrowHash = Animator.StringToHash("Grow");
        protected static readonly int DissipateHash = Animator.StringToHash("Dissipate");

        protected const float crossFadeDuration = 0.1f;

        public MushroomState(MushroomObject mushroomObject, Animator animator)
        {
            this.mushroomObject = mushroomObject;
            this.animator = animator;
        }

        public virtual void OnEnter() { }

        public virtual void Update() { }

        public virtual void FixedUpdate() { }
        
        public virtual void OnExit() { }
    }

    public class GrowState : MushroomState
    {
        private CountdownTimer growTimer;

        public GrowState(MushroomObject mushroomObject, Animator animator) : base(mushroomObject, animator) { }

        public override void OnEnter()
        {
            // Start the animation and set the grow timer
            animator.CrossFade(GrowHash, crossFadeDuration);

            // Create a timer that sets the mushroom to Idle after the animation
            growTimer = new CountdownTimer(animator.GetCurrentAnimatorStateInfo(0).length);
            growTimer.OnTimerStop += () => mushroomObject.StopGrowing();
            growTimer.Start();
        }

        public override void OnExit()
        {
            // Dispose the Timer
            growTimer.Dispose();
        }
    }

    public class IdleState : MushroomState
    {
        public IdleState(MushroomObject mushroomObject, Animator animator) : base(mushroomObject, animator) { }

        public override void OnEnter()
        {
            animator.CrossFade(IdleHash, crossFadeDuration);
        }
    }

    public class BounceState : MushroomState
    {
        private CountdownTimer bounceTimer;
        private readonly MushroomSFX sfx;

        public BounceState(MushroomObject mushroomObject, Animator animator, MushroomSFX sfx) : base(mushroomObject, animator) 
        {
            this.sfx = sfx;
        }

        public override void OnEnter()
        {
            // Cross fade into the animation
            animator.CrossFade(BounceHash, crossFadeDuration);

            // Create a timer that sets the mushroom to idle after the animation
            bounceTimer = new CountdownTimer(animator.GetCurrentAnimatorStateInfo(0).length);
            bounceTimer.OnTimerStop += () => mushroomObject.StopBouncing();
            bounceTimer.Start();

            // Play the Mushroom particles
            mushroomObject.UnleashParticles();

            // Play the Mushroom bounce sound
            sfx.Bounce();
        }

        public override void OnExit()
        {
            // Dispose the Timer
            bounceTimer.Dispose();
        }
    }

    public class DissipateState : MushroomState
    {
        private readonly MushroomSFX sfx;
        private CountdownTimer dissipateTimer;

        public DissipateState(MushroomObject mushroomObject, Animator animator, MushroomSFX sfx) : base(mushroomObject, animator)
        {
            this.sfx = sfx;
        }

        ~DissipateState()
        {
            // Dispose of the timer
            dissipateTimer.Dispose();
        }

        public override void OnEnter()
        {
            // Play the sound
            sfx.Dissipate();

            // Cross fade into the animation
            animator.CrossFade(DissipateHash, crossFadeDuration);

            // Create a timer that sets the mushroom to idle after the animation
            dissipateTimer = new CountdownTimer(0.5f);
            dissipateTimer.OnTimerStop += () => mushroomObject.DestroyMushroom();
            dissipateTimer.Start();
        }

        public override void OnExit()
        {
            // Dispose the Timer
            dissipateTimer.Dispose();
        }
    }
}
