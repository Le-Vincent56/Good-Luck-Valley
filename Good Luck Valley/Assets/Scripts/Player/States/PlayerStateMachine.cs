using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Player.Animation;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Player.States;
using UnityEngine;

namespace GoodLuckValley
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private PlayerController controller;
        private AnimationController animator;

        private StateMachine superMachine;

        [SerializeField] private bool debug;
        [SerializeField] private string currentSuperState;
        [SerializeField] private string currentSubState;

        private void Awake()
        {
            // Get components
            controller = GetComponent<PlayerController>();
            animator = GetComponentInChildren<AnimationController>();

            // Initialize the animator
            animator.Initialize(controller);

            // Initialize the super State Machine
            superMachine = new StateMachine();

            // Create states
            GroundedState grounded = new GroundedState(controller, animator);
            WallState wallSliding = new WallState(controller, animator);
            JumpState jumping = new JumpState(controller, animator);
            BounceState bouncing = new BounceState(controller, animator);
            FallState falling = new FallState(controller, animator);

            // Define state transitions
            superMachine.At(grounded, jumping, new FuncPredicate(() => !controller.Collisions.Grounded && !controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));
            superMachine.At(grounded, falling, new FuncPredicate(() => !controller.Collisions.Grounded && controller.RB.linearVelocity.y < 0));
            superMachine.At(grounded, bouncing, new FuncPredicate(() => controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));

            superMachine.At(wallSliding, grounded, new FuncPredicate(() => !controller.WallJump.IsOnWall && controller.Collisions.Grounded));
            superMachine.At(wallSliding, jumping, new FuncPredicate(() => !controller.WallJump.IsOnWall && controller.RB.linearVelocity.y > 0));

            superMachine.At(jumping, grounded, new FuncPredicate(() => controller.Collisions.Grounded));
            superMachine.At(jumping, wallSliding, new FuncPredicate(() => controller.WallJump.IsOnWall));
            superMachine.At(jumping, falling, new FuncPredicate(() => !controller.Collisions.Grounded && controller.RB.linearVelocity.y < 0));
            superMachine.At(jumping, bouncing, new FuncPredicate(() => controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));

            superMachine.At(bouncing, grounded, new FuncPredicate(() => controller.Collisions.Grounded));
            superMachine.At(bouncing, wallSliding, new FuncPredicate(() => controller.WallJump.IsOnWall));
            superMachine.At(bouncing, falling, new FuncPredicate(() => controller.RB.linearVelocity.y < 0));

            superMachine.At(falling, grounded, new FuncPredicate(() => controller.Collisions.Grounded));
            superMachine.At(falling, wallSliding, new FuncPredicate(() => controller.WallJump.IsOnWall));
            superMachine.At(falling, bouncing, new FuncPredicate(() => controller.Bounce.Bouncing && controller.RB.linearVelocity.y > 0));

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
