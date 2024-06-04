using GoodLuckValley.Mushroom;

namespace GoodLuckValley.Patterns.Visitor
{
    public interface IVisitor
    {
        void Visit(ShroomTimer shroomTimer);
    }
}