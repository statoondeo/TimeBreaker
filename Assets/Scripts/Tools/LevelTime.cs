using System;

[Serializable]
public class LevelTime
{
	public float Time { get; private set; }

	public LevelTime(float time) => Time = time;
}
