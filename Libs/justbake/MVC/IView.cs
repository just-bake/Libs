namespace Libs.justbake.MVC
{
    public interface IView
    {
        IController Controller { get; }
        
        void RegisterBindings();
        void DeregisterBindings();
    }
}