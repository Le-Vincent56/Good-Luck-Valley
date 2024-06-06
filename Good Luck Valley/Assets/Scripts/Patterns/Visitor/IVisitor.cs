using GoodLuckValley.Mushroom;
using UnityEngine;

namespace GoodLuckValley.Patterns.Visitor
{
    public interface IVisitor
    {
        void Visit<T>(T visitable) where T : Component, IVisitable;
    }
}