using GoodLuckValley.Events;
using GoodLuckValley.Events.World;
using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.World.Triggers
{
    public class LightningTrigger : EnterExitTrigger
    {
        public override void OnEnter(PlayerController controller)
        {
            EventBus<StrikeLightning>.Raise(new StrikeLightning());
        }

        public override void OnExit(PlayerController controller) { }
    }
}
