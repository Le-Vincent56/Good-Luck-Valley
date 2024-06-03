using GoodLuckValley.Patterns.Commands;

namespace GoodLuckValley.World.Decomposables
{
    public interface IDecomposable
    {
        /// <summary>
        /// Queue a command for the Decomposable
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IDecomposable> command);

        /// <summary>
        /// Execute the Interaction
        /// </summary>
        public void ExecuteCommand();
    }

}