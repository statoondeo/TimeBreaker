using System;
using System.Collections.Generic;

[Serializable]
public class LevelsTimes
{
	public Dictionary<string, LevelTimes> LevelTimes;

	public LevelsTimes() => LevelTimes = new Dictionary<string, LevelTimes>();
}
