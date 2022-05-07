using System;
using System.Collections.Generic;

[Serializable]
public class LevelTimes
{
	public List<LevelTime> Times;
	public bool Locked;
	public float BestTime;
	public int Tries;

	public LevelTimes()
	{
		Times = new List<LevelTime>();
		BestTime = float.MaxValue;
		Locked = true;
		Tries = 0;
	}
}
