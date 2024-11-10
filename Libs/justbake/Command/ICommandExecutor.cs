using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libs.justbake.Command
{
    public interface ICommandExecutor
    {
        void QueueCommand(ICommand command);
        bool TryDequeCommand(out ICommand command);
        bool TryDoCommands(out List<Exception> exceptions, int count = -1);
        bool TryUndoCommands(out List<Exception> exceptions, int count = -1);
        Result DoCommands(int count = -1);
        Result UndoCommands(int count = -1);
        Task<Result> DoCommandsAsync(int count = -1);
        Task<Result> UndoCommandsAsync(int count = -1);
    }
}