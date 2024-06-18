using System.Collections;

namespace GoodLuckValley.World.Decomposables
{
    public interface IDecomposable
    {
        void Decompose(float decomposeTime);

        void Recompose(float decomposeTime);

        int GetIndex();
    }
}