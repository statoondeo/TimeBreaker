using System;
using System.Collections.Generic;

public partial class EventsService : IService
{
	private readonly Dictionary<Events, EventModel> EventsAtlas;
	private readonly EventModelArgPool<OnBallCollidedEventArg> OnBallCollidedEventArgPool;
	private readonly Queue<Tuple<Events, EventModelArg>> EventsQueue;
	private bool PerformQueueWip;

	public EventsService()
	{
		EventsAtlas = new Dictionary<Events, EventModel>();
		EventsQueue = new Queue<Tuple<Events, EventModelArg>>();
		OnBallCollidedEventArgPool = new EventModelArgPool<OnBallCollidedEventArg>(20, 0.1f);
		Events[] events = (Events[])Enum.GetValues(typeof(Events));
		for (int i = 0, nbItems = events.Length; i < nbItems; i++) EventsAtlas.Add(events[i], new EventModel());
		PerformQueueWip = false;
	}

	private EventModel GetEvent(Events eventName) => EventsAtlas[eventName];

	private void PerformQueue()
	{
		PerformQueueWip = true;
		while(EventsQueue.Count > 0)
		{
			Tuple<Events, EventModelArg> eventTupe = EventsQueue.Dequeue();
			GetEvent(eventTupe.Item1).Raise(eventTupe.Item2);
		}
		PerformQueueWip = false;
	}


	public OnBallCollidedEventArg GetOnBallCollidedEventArg() => OnBallCollidedEventArgPool.Get();

	public void Raise(Events eventName) => Raise(eventName, EventModelArg.Empty);

	public void Raise(Events eventName, EventModelArg eventArg)
	{
		EventsQueue.Enqueue(new Tuple<Events, EventModelArg>(eventName, eventArg));
		if (!PerformQueueWip) PerformQueue();
	}

	//public void Raise(Events eventName, EventModelArg eventArg) => GetEvent(eventName).Raise(eventArg);

	public void Register(Events eventToListen, Action<EventModelArg> callback) => GetEvent(eventToListen).RegisterListener(callback);

	public void UnRegister(Events eventToListen, Action<EventModelArg> callback) => GetEvent(eventToListen).UnregisterListener(callback);
}
