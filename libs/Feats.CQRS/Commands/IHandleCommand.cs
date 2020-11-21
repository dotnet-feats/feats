public namespace Feats.CQRS.Queries
{
    public interface IHandleCommand<TCommand> 
        where TCommandy : ICommand
    {
        Task Handle(TCommand command);
    }
}