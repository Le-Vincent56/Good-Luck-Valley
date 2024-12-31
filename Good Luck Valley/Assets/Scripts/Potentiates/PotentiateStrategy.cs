using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Potentiates
{
    public abstract class PotentiateStrategy
    {
        public abstract bool Potentiate(PlayerController controller, PotentiateHandler handler);
        public abstract void Deplete();
    }
}
