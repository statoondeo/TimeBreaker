using UnityEngine;

public class EventModelArgPool<T> where T : EventModelArg, new()
{
	private int CurrentIndex;
	private readonly int Capacity;
	private readonly T[] PooledObjects;
	private readonly float Ttl;

	public EventModelArgPool(int capacity, float ttl)
	{
		Capacity = capacity;
		Ttl = ttl;
		CurrentIndex = 0;
		PooledObjects = new T[Capacity];
		for (int i = 0; i < Capacity; i++)
		{
			PooledObjects[i] = new T();
			PooledObjects[i].TtlEndTime = 0.0f;
		}
	}

	public T Get()
	{
		float currentTime = Time.time;
		float endTime = Time.time + Ttl;
		int nbRead = 0;
		T newObject = PooledObjects[CurrentIndex];
		while ((nbRead < Capacity) && (newObject.TtlEndTime > currentTime))
		{
			CurrentIndex = (CurrentIndex + 1) % Capacity;
			newObject = PooledObjects[CurrentIndex];
			nbRead++;
		}
		if (nbRead >= Capacity)
		{
			Debug.LogError("Pool mal dimensionné");
			return (null);
		}
		newObject.TtlEndTime = endTime;
		return (newObject);
	}
}
