using UnityEngine;

public class TimeWatch
{
	public static TimeWatch StartNew() => new TimeWatch(true);

	private float CumuledTime;
	private float StartTime;
	private bool Running;

	public TimeWatch(bool start = false)
	{
		Reset();
		if (start) Start();
	}

	public float Elapsed => Mathf.Round(CumuledTime + (Running ? 1000.0f * (Time.time - StartTime) : 0.0f));

	public void Reset() 
	{
		CumuledTime = 0.0f;
		StartTime = 0.0f;
		Running = false;
	}

	public void Start() 
	{
		if (Running) return;
		StartTime = Time.time;
		Running = true;
	}
	public void Stop()
	{
		if (!Running) return;
		CumuledTime += 1000.0f * (Time.time - StartTime);
		Running = false;
	}
}
