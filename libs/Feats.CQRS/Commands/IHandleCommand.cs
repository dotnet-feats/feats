using System.Threading.Tasks;

namespace Feats.CQRS.Commands
{
    public interface IHandleCommand<TCommand> 
        where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}