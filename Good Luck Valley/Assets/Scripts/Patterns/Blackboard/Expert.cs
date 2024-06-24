namespace GoodLuckValley.Patterns.Blackboard
{
    public interface IExpert
    {
        int GetInsistence(Blackboard blackboard);
        void Execute(Blackboard blackboard);
    }
}