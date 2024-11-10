using System;
using System.Threading.Tasks;

namespace Libs.justbake.Command
{
    public interface ICommand : IEquatable<ICommand>
    {
        public void Do();            // Synchronous version of Do method
        public void Undo();          // Synchronous version of Undo method
    }
}