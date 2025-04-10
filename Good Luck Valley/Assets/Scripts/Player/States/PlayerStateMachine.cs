using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Audio;
using GoodLuckValley.Particles;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Development;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Player.States;
using UnityEngine;

namespace GoodLuckValley
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private PlayerController controller;
        private AnimationController animator;
        private PlayerParticleController particles;
        private PlayerSFX sfx;
        private DevelopmentTools devTools;

        private StateMachine superMachine;

        [SerializeField] private bool debug;
        [SerializeField] private string currentSuperState;
        [SerializeField] private string currentSubState;

        private void Awake()
        {
            // Get components
            controller = GetComponent<PlayerController>();
            animator = GetComponentInChildren<AnimationController>();
            particles = GetComponentInChildren<PlayerParticleController>();
            sfx = GetComponentInChildren<PlayerSFX>();
            devTools = GetComponent<DevelopmentTools>();

            // Initialize the animator
            animator.Initialize(controller);
        }

        private void Start()
        {
            // Initialize the super State Machine
            superMachine = new StateMachine();

            // Create states
            GroundedState grounded = new GroundedState(controller, animator, particles, sfx, superMachine);
            WallState wallSliding = new WallState(controller, animator, particles, sfx);
            JumpState jumping = new JumpState(controller, animator, particles, sfx);
            BounceState bouncing = new BounceState(controller, animator, particles, sfx);
            FallState falling = new FallState(controller, animator, particles, sfx);
            NoClipState noClip = new NoClipState(controller, animator, particles, sfx);
            SlideState sliding = new SlideState(controller, animator, particles, sfx);

            // Define state transitions
            superMachine.At(grounded, jumping, new FuncPredicate(() => !controller.Collisions.Grounded && !controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));
            superMachine.At(grounded, falling, new FuncPredicate(() => !controller.Collisions.Grounded && controller.RB.linearVelocity.y < 0));
            superMachine.At(grounded, bouncing, new FuncPredicate(() => controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));
            superMachine.At(grounded, sliding, new FuncPredicate(() => controller.Collisions.IsSliding));

            superMachine.At(wallSliding, grounded, new FuncPredicate(() => !controller.WallJump.IsOnWall && controller.Collisions.Grounded));
            superMachine.At(wallSliding, jumping, new FuncPredicate(() => !controller.WallJump.IsOnWall && controller.RB.linearVelocity.y > 0));
            superMachine.At(wallSliding, falling, new FuncPredicate(() => !controller.WallJump.IsOnWall && controller.RB.linearVelocity.y < 0));

            superMachine.At(jumping, grounded, new FuncPredicate(() => controller.Collisions.Grounded));
            superMachine.At(jumping, wallSliding, new FuncPredicate(() => controller.WallJump.IsOnWall));
            superMachine.At(jumping, falling, new FuncPredicate(() => !controller.Collisions.Grounded && controller.RB.linearVelocity.y < 0));
            superMachine.At(jumping, bouncing, new FuncPredicate(() => controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));
            superMachine.At(jumping, jumping, new FuncPredicate(() => controller.Jump.IsJumping && controller.RB.linearVelocity.y > 0));

            superMachine.At(bouncing, grounded, new FuncPredicate(() => controller.Collisions.Grounded));
            superMachine.At(bouncing, wallSliding, new FuncPredicate(() => controller.WallJump.IsOnWall));
            superMachine.At(bouncing, jumping, new FuncPredicate(() => controller.Jump.IsJumping && controller.RB.linearVelocity.y > 0));
            superMachine.At(bouncing, falling, new FuncPredicate(() => !controller.Bounce.Bouncing && controller.RB.linearVelocity.y < 0));

            superMachine.At(falling, grounded, new FuncPredicate(() => controller.Collisions.Grounded));
            superMachine.At(falling, wallSliding, new FuncPredicate(() => controller.WallJump.IsOnWall));
            superMachine.At(falling, jumping, new FuncPredicate(() => controller.Jump.IsJumping && controller.RB.linearVelocity.y > 0));
            superMachine.At(falling, bouncing, new FuncPredicate(() => controller.Bounce.Bouncing));
            superMachine.At(falling, sliding, new FuncPredicate(() => controller.Collisions.IsSliding));

            superMachine.Any(noClip, new FuncPredicate(() => devTools.NoClip));
            superMachine.At(noClip, grounded, new FuncPredicate(() => !devTools.NoClip && controller.Collisions.Grounded));
            superMachine.At(noClip, falling, new FuncPredicate(() => !devTools.NoClip && !controller.Collisions.Grounded));
            superMachine.At(noClip, bouncing, new FuncPredicate(() => !devTools.NoClip && controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));
            superMachine.At(noClip, wallSliding, new FuncPredicate(() => !devTools.NoClip && controller.WallJump.IsOnWall));

            superMachine.At(sliding, falling, new FuncPredicate(() => !controller.Collisions.Grounded && !controller.Collisions.IsSliding));
            superMachine.At(sliding, grounded, new FuncPredicate(() => controller.Collisions.Grounded && !controller.Collisions.IsSliding));
            superMachine.At(sliding, bouncing, new FuncPredicate(() => controller.Bounce.Bouncing));

            // Set the initial state
            superMachine.SetState(falling);
        }

        public void TickUpdate()
        {
            // Update the super State Machine
            superMachine.Update();

            // Exit case - not debugging
            if (!debug) return;

            // Debug the states
            currentSuperState = superMachine.GetState().ToString().Replace("GoodLuckValley.Player.States.", "");
            currentSubState = (superMachine.GetState() as SuperState).subStates?.GetState().ToString().Replace("GoodLuckValley.Player.States.", "");
        }

        public void TickFixedUpdate()
        {
            // Update the super State Machine
            superMachine.FixedUpdate();
        }
    }
}
