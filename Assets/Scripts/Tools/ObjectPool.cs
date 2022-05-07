using System;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
	private int CurrentIndex;
	private readonly int Capacity;
	private readonly T[] PooledObjects;

	public ObjectPool(Func<T> creationMethod, int capacity)
	{
		Capacity = capacity;
		CurrentIndex = 0;
		PooledObjects = new T[Capacity];
		for (int i = 0; i < Capacity; i++)
		{
			PooledObjects[i] = creationMethod();
			PooledObjects[i].gameObject.SetActive(false);
		}
	}

	public T Get(Vector3 position, Quaternion rotation)
	{
		int nbRead = 0;
		T newObject = PooledObjects[CurrentIndex];
		while ((nbRead < Capacity) && newObject.gameObject.activeSelf)
		{
			CurrentIndex = (CurrentIndex + 1) % Capacity;
			newObject = PooledObjects[CurrentIndex];
			nbRead++;
		}
		if (nbRead >= Capacity)
		{
			Debug.LogError("Pool mal dimensionné/" + PooledObjects[0].name);
			return (null);
		}
		newObject.gameObject.SetActive(true);
		newObject.transform.SetPositionAndRotation(position, rotation);
		return (newObject);
	}
}
