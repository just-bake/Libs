using System;

namespace Libs.justbake.Command.Commands
{
    public class ActionCommand : ICommand
    {
        private readonly Guid _id;
        
        private readonly Action _doAction; 
        private readonly Action _undoAction;

        public ActionCommand(Action doAction, Action undoAction)
        {
            _id = Guid.NewGuid();
            
            _undoAction = undoAction;
            _doAction = doAction;
        }
        
        public ActionCommand(Action doAction)
        {
            _id = Guid.NewGuid();
            
            _undoAction = () => {};
            _doAction = doAction;
        }

        public void Do() => _doAction.Invoke();
        public void Undo() => _undoAction.Invoke();
        
        public bool Equals(ICommand other)
        {
            if (other is ActionCommand otherCommand)
                return _id.Equals(otherCommand._id); // Assuming _id is a defined identifier in ActionCommand
            return false;
        }
        
        public override bool Equals(object? obj)
        {
            ActionCommand? other = obj as ActionCommand;
            return other != null && _id == other._id;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}