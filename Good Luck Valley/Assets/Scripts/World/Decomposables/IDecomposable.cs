using GoodLuckValley.Patterns.Commands;

namespace GoodLuckValley.World.Decomposables
{
    public interface IDecomposable
    {
        void Decompose();

        void Recompose();
    }
}