namespace GoodLuckValley.Patterns.Visitor
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }
}