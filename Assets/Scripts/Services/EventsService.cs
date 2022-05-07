using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EventsService
{

	private readonly Dictionary<Events, EventModel> EventsAtlas;
	private readonly EventModelArgPool<OnBallCollidedEventArg> OnBallCollidedEventArgPool;

	public EventsService()
	{
		EventsAtlas = new Dictionary<Events, EventModel>();
		OnBallCollidedEventArgPool = new EventModelArgPool<OnBallCollidedEventArg>(20, 0.1f);
		Events[] events = (Events[])Enum.GetValues(typeof(Events));
		for (int i = 0, nbItems = events.Length; i < nbItems; i++) EventsAtlas.Add(events[i], new EventModel());
	}

	private EventModel GetEvent(Events eventName) => EventsAtlas[eventName];

	public OnBallCollidedEventArg GetOnBallCollidedEventArg() => OnBallCollidedEventArgPool.Get();

	public void Raise(Events eventName) => Raise(eventName, EventModelArg.Empty);

	public void Raise(Events eventName, EventModelArg eventArg) => GetEvent(eventName).Raise(eventArg);

	public void Register(Events eventToListen, Action<EventModelArg> callback) => GetEvent(eventToListen).RegisterListener(callback);

	public void UnRegister(Events eventToListen, Action<EventModelArg> callback) => GetEvent(eventToListen).UnregisterListener(callback);
}
