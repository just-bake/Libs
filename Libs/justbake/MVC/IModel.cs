namespace Libs.justbake.MVC
{
    public interface IModel
    {
        IController Controller { get; }
        
        void RegisterBindings();
        void DeregisterBindings();
    }
}