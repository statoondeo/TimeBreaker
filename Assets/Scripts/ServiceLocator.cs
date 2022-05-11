using System;
using System.Collections.Generic;

public class ServiceLocator
{
	private readonly Dictionary<Type, IService> ServicesDictionary;

	public ServiceLocator() => ServicesDictionary = new Dictionary<Type, IService>();

	public T Get<T>() where T : class => ServicesDictionary[typeof(T)] as T;

	public T Register<T>(T service) where T : IService
	{
		ServicesDictionary.Add(typeof(T), service);
		return (service);
	}

	public void UnRegister<T>(T service) where T : IService => ServicesDictionary.Remove(typeof(T));
}

