namespace WebUI.Events
{
    public interface IEventHandler
    {
        object HandleEvent(Event @event, string element, ulong handlerId);
    }
}
