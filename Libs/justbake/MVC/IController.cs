namespace Libs.justbake.MVC
{
    public interface IEvent
    {
    }

    public interface IModelEvent : IEvent
    {
    }

    public interface IViewEvent : IEvent
    {
    }

    public interface IEventListener<in TEvent> where TEvent : IEvent
    {
        void OnEvent(TEvent eventInstance);
    }

    public interface IEventRaiser<TEvent> where TEvent : IEvent
    {
        void RaiseEvent(TEvent eventInstance);
        void AddEventListener(IEventListener<TEvent> listener);
        void RemoveEventListener(IEventListener<TEvent> listener);
    }

    public interface IController : IEventRaiser<IModelEvent>, IEventRaiser<IViewEvent>
    {
        IView View { get; set; }
        IModel Model { get; set; }
    }
}