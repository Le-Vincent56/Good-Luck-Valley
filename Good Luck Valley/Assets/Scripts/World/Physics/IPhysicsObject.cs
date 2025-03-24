
namespace GoodLuckValley.World.Physics
{
    public interface IPhysicsObject
    {
        public void TickUpdate(float delta, float time);
        public void TickFixedUpdate(float delta);
    }
}
