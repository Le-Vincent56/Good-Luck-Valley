using UnityEngine;

namespace GoodLuckValley.Patterns.Visitor
{
    public interface IVisitable
    {
        void Accept<T>(T visitor) where T : Component, IVisitor;
    }
}