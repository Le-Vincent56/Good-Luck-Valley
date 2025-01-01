using GoodLuckValley.World.Physics;

namespace GoodLuckValley.Interactables.Fireflies.States
{
    public class GroupLocomotionState : GroupState
    {
        private readonly PhysicsOrchestrator physicsOrchestrator;

        public GroupLocomotionState(Fireflies controller, PhysicsOrchestrator physicsOrchestrator) : base(controller) 
        {
            this.physicsOrchestrator = physicsOrchestrator;
        }

        ~GroupLocomotionState()
        {
            // Deregister from the Physics Orchestrator
            physicsOrchestrator.Deregister(controller);
        }

        public override void OnEnter()
        {
            // Register to the Physics Orchestrator
            physicsOrchestrator.Register(controller);
        }

        public override void OnExit()
        {
            // Deregister from the Physics Orchestrator
            physicsOrchestrator.Deregister(controller);
        }
    }
}
