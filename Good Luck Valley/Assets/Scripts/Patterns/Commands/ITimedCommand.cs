using System.Threading.Tasks;

namespace GoodLuckValley.Patterns.Commands
{
    public interface ITimedCommand<T>
    {
        Task Execute();
    }
}