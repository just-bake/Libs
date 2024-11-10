using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libs.justbake.Command
{
    /// <summary>
    /// Represents a command executor that can queue, process, and undo commands.
    /// </summary>
    /// <remarks>
    /// This class provides methods for queuing, executing, and undoing commands in a thread-safe manner.
    /// </remarks>
    /// <seealso cref="ICommandExecutor" />
    /// <seealso cref="ICommand" />
    /// <seealso cref="Result" />
    /// <seealso cref="ConcurrentQueue{T}" />
    /// <seealso cref="List{T}" />
    /// <seealso cref="Task" />
    /// <seealso cref="Func{TResult}" />
    /// <seealso cref="Action" />
    /// <seealso cref="Environment" />
    /// <seealso cref="IEnumerable{T}" />
    /// <seealso cref="ConcurrentQueue{T}" />
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="ConcurrentQueue{T}" />
    /// <seealso cref="Exception" />
    /// <seealso cref="string" />
    /// <seealso cref="IEquatable{T}" />
    /// <seealso cref="Task{Result}" />
    /// <seealso cref="GetResult" />
    /// <seealso cref="CommandExecutor.ProcessCommands" />
    public class CommandExecutor : ICommandExecutor
    {
        /// <summary>
        /// Represents a queue for holding commands to be executed by a CommandExecutor.
        /// </summary>
        private readonly ConcurrentQueue<ICommand> _commandQueue = new ConcurrentQueue<ICommand>();

        /// <summary>
        /// Represents the history of the command queue in a thread-safe manner.
        /// </summary>
        private readonly ConcurrentQueue<ICommand> _commandQueueHistory = new ConcurrentQueue<ICommand>();

        /// <summary>
        /// Represents the list of exceptions that occurred during command execution. </summary>
        /// /
        private readonly List<Exception> _exceptions = new List<Exception>();
        private readonly List<Task> _tasks = new List<Task>();

        /// <summary>
        /// Queues a command for execution.
        /// </summary>
        /// <param name="command">The command to be queued for execution.</param>
        public void QueueCommand(ICommand command)
        {
            _commandQueue.Enqueue(command);
        }

        /// <summary>
        /// Tries to dequeue a command from the command queue.
        /// </summary>
        /// <param name="command">The dequeued command, if successful.</param>
        /// <returns>True if a command was dequeued successfully; otherwise, false.</returns>
        public bool TryDequeCommand(out ICommand command)
        { 
            return _commandQueue.TryDequeue(out command);
        }

        /// <summary>
        /// Tries to execute a specified number of commands from the command queue.
        /// </summary>
        /// <param name="exceptions">List of exceptions that occurred during command execution.</param>
        /// <param name="count">Number of commands to execute. Default is all commands in the queue.</param>
        /// <returns>True if all commands executed successfully, otherwise false.</returns>
        public bool TryDoCommands(out List<Exception> exceptions, int count = -1)
        {
            exceptions = ProcessCommands(count, command => command.Do, _commandQueue, _commandQueueHistory.Enqueue);
            return _exceptions.Count == 0;
        }

        /// <summary>
        /// Tries to undo a specified number of commands in the command queue.
        /// </summary>
        /// <param name="exceptions">List of exceptions that occurred during the undo process.</param>
        /// <param name="count">Number of commands to undo. Default is -1 (undo all commands).</param>
        /// <returns>True if the undo operation was successful without any exceptions, otherwise false.</returns>
        public bool TryUndoCommands(out List<Exception> exceptions, int count = -1)
        {
            exceptions = ProcessCommands(count, command => command.Undo, _commandQueue);
            return _exceptions.Count == 0;
        }

        /// <summary>
        /// Executes a specified number of commands from the command queue.
        /// </summary>
        /// <param name="count">The number of commands to execute. Set to -1 to execute all commands in the queue.</param>
        /// <returns>Returns the Result of the command execution operation.</returns>
        /// <remarks>
        /// If the count is set to -1, all commands in the queue will be executed.
        /// The Result structure contains information about the success of the operation and any error messages encountered.
        /// </remarks>
        public Result DoCommands(int count = -1)
        {
            ProcessCommands(count, command => command.Do, _commandQueue, _commandQueueHistory.Enqueue);
            return GetResult();
        }

        /// <summary>
        /// Undoes the specified number of commands from the command history.
        /// </summary>
        /// <param name="count">The number of commands to undo. Default is -1 to undo all available commands.</param>
        /// <returns>The result of undoing the commands, indicating success or failure along with any error messages.</returns>
        public Result UndoCommands(int count = -1)
        {
            ProcessCommands(count, command => command.Undo, _commandQueueHistory);
            return GetResult();
        }

        /// <summary>
        /// Processes a specified number of commands from the command queue using the provided callback function.
        /// </summary>
        /// <param name="count">The number of commands to process. If set to -1, all commands in the queue will be processed.</param>
        /// <param name="getCommandAction">A Func delegate that specifies the action to retrieve from each command.</param>
        /// <param name="commandQueue">The queue containing the commands to be processed.</param>
        /// <param name="postCommandAction">An optional action to be performed after each command is processed.</param>
        /// <returns>A list of Exceptions encountered during command processing.</returns>
        private List<Exception> ProcessCommands(int count, Func<ICommand, Action> getCommandAction, ConcurrentQueue<ICommand> commandQueue, Action<ICommand>? postCommandAction = null)
        {
            if (count == -1) count = commandQueue.Count;
            _exceptions.Clear();

            for (int i = 0; i < count; i++)
            {
                if (!commandQueue.TryDequeue(out ICommand command)) continue;
                TryCommand(getCommandAction(command));
                postCommandAction?.Invoke(command);
            }

            return _exceptions;
        }

        /// <summary>
        /// Executes a specified number of commands asynchronously.
        /// </summary>
        /// <param name="count">The number of commands to execute asynchronously. Default is -1 to execute all commands in the queue.</param>
        /// <returns>Returns a Task with the Result of the execution.</returns>
        public async Task<Result> DoCommandsAsync(int count = -1)
        {
            return await ProcessCommandsAsync(command => command.Do, count);
        }

        /// <summary>
        /// Undoes the specified number of commands asynchronously.
        /// </summary>
        /// <param name="count">The number of commands to undo. Default is -1 which means undo all.</param>
        /// <returns>A <see cref="Task{Result}"/> representing the asynchronous undo operation.</returns>
        public async Task<Result> UndoCommandsAsync(int count = -1)
        {
            return await ProcessCommandsAsync(command => command.Undo, count);
        }

        /// <summary>
        /// Processes a specified number of commands asynchronously.
        /// </summary>
        /// <param name="getAction">Function that returns the action to perform on each command.</param>
        /// <param name="count">The number of commands to process. If set to -1, all available commands will be processed.</param>
        /// <param name="commandAction">Action to perform after processing each command.</param>
        /// <returns>A task that represents the asynchronous operation and returns the result of the processing.</returns>
        private async Task<Result> ProcessCommandsAsync(Func<ICommand, Action> getAction, int count = -1, Action<ICommand>? commandAction = null)
        {
            if (count == -1) count = _commandQueue.Count;
                
            _tasks.Clear();
                
            for (int i = 0; i < count; i++)
            {
                if(!_commandQueue.TryDequeue(out ICommand command)) continue;
                _tasks.Add(new Task(() =>
                {
                    TryCommand(getAction(command));
                }));

                commandAction?.Invoke(command);
            }
                
            await Task.WhenAll(_tasks);
            return GetResult();
        }

        /// <summary>
        /// Attempts to execute a command action and captures any exceptions that occur.
        /// </summary>
        /// <param name="commandAction">The action to execute as a command.</param>
        private void TryCommand(Action commandAction)
        {
            try
            {
                commandAction.Invoke();
            }
            catch (Exception e)
            {
                _exceptions.Add(e);
            }
        }

        /// <summary>
        /// Retrieves the result of the command execution.
        /// </summary>
        /// <returns>A <see cref="Result"/> object indicating the success status and any error messages.</returns>
        private Result GetResult()
        {
            string errorMessages = string.Join(Environment.NewLine, _exceptions.Select(e => $"{e.Source}: {e.Message}"));
            Result result = new Result(_exceptions.Count == 0, errorMessages);
            _exceptions.Clear();

            return result;
        }
    }
}