using System;

public class EventModel
{
    private Action<EventModelArg> Listeners;

    public void Raise(EventModelArg eventArg) => Listeners?.Invoke(eventArg);

    public void RegisterListener(Action<EventModelArg> callback) => Listeners += callback;

    public void UnregisterListener(Action<EventModelArg> callback) => Listeners -= callback;
}
