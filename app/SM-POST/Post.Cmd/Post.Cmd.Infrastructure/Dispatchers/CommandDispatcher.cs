using CQRS.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers;
public class CommandDispatcher : ICommandDispatcher
{
    private Dictionary<Type, Func<BaseCommand, Task>> _handlers = new();
    public void RegisterHandler<TCommand>(Func<TCommand, Task> handler) where TCommand : BaseCommand
    {
        if (_handlers.ContainsKey(typeof(TCommand)))
        {
            throw new IndexOutOfRangeException("You cannot register the same command handler twice");
        }
        _handlers.Add(typeof(TCommand), baseCommand => handler((TCommand)baseCommand));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out var handler))
        {
            await handler(command);
        }
        else
            throw new ArgumentNullException(nameof(handler), "Command handler not registered yet.");
    }
}
