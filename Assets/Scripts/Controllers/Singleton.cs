using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance != null) return (instance);

			instance = FindObjectOfType<T>();
			if (instance != null) return (instance);

			GameObject obj = new GameObject();
			obj.name = typeof(T).Name;
			obj.hideFlags = HideFlags.DontSave;
			instance = obj.AddComponent<T>();

			return instance;
		}
	}

	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(gameObject);
			return;
		}
		Destroy(gameObject);
	}
}
