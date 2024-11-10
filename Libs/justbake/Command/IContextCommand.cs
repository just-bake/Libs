namespace Libs.justbake.Command
{
    public interface IContextCommand<out T> : ICommand
    {
        T Context { get; }
    }
}