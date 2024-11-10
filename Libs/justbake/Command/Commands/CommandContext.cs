using System;

namespace Libs.justbake.Command.Commands
{
    /// <summary>
    /// Represents a command context that stores a specific context type and associated actions to perform and undo the command.
    /// </summary>
    /// <typeparam name="T">The type of the context.</typeparam>
    public class CommandContext<T> : IContextCommand<T>
    {
        /// <summary>
        /// Represents a generic command context that stores the context of a command along with the actions to perform and undo that command.
        /// </summary>
        public T Context { get; set; }

        /// <summary>
        /// Represents the unique identifier assigned to a CommandContext instance.
        /// </summary>
        private readonly Guid _id;

        /// <summary>
        /// Represents a command context with a specific type of context.
        /// </summary>
        /// <typeparam name="T">The type of the context.</typeparam>
        private readonly Action<T> _doAction;

        /// <summary>
        /// Represents an undo action associated with a specific command context.
        /// </summary>
        private readonly Action<T> _undoAction;

        /// <summary>
        /// Represents a command context that encapsulates an action to be executed and undone within a specific context.
        /// </summary>
        /// <typeparam name="T">The type of the context.</typeparam>
        public CommandContext(T context, Action<T> doAction, Action<T> undoAction)
        {
            _id = Guid.NewGuid();

            Context = context;
            _doAction = doAction;
            _undoAction = undoAction;
        }

        /// <summary>
        /// Executes the action associated with this CommandContext instance.
        /// </summary>
        /// <remarks>
        /// This method invokes the action specified in the constructor on the context object passed during initialization.
        /// </remarks>
        public void Do() => _doAction.Invoke(Context);

        /// <summary>
        /// Undoes the action that was previously executed by the command.
        /// </summary>
        /// <remarks>
        /// This method is called to reverse the action performed by the command during the execution phase.
        /// </remarks>
        /// <seealso cref="ICommand"/>
        public void Undo() => _undoAction.Invoke(Context);

        /// <summary>
        /// Determines whether the current CommandContext object is equal to another ICommand object.
        /// </summary>
        /// <param name="other">The ICommand object to compare with the current CommandContext.</param>
        /// <returns>True if the current CommandContext object is equal to the other object; otherwise, false.</returns>
        public bool Equals(ICommand other)
        {
            if (other is CommandContext<T> otherCommand)
                return _id.Equals(otherCommand._id);
            return false;
        }

        /// <summary>
        /// Determines whether the current CommandContext&lt;T&gt; instance is equal to the specified ICommand.
        /// </summary>
        /// <param name="obj">The ICommand to compare with the current CommandContext&lt;T&gt; instance.</param>
        /// <returns>true if the specified ICommand is a CommandContext&lt;T&gt; and its GUID identifier equals the current instance's GUID identifier; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            CommandContext<T>? other = obj as CommandContext<T>;
            return other != null && _id == other._id;
        }

        /// <summary>
        /// Returns the hash code value for this CommandContext based on its unique identifier.
        /// </summary>
        /// <returns/>The hash code value for this CommandContext instance.
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}