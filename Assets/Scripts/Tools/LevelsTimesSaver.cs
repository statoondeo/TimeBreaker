using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class LevelsTimesSaver
{
	private static readonly string SAVE_FILENAME = "/SavedData.dat";
	public LevelsTimes LevelsTimes { get; private set; }

	public LevelsTimesSaver() => Load();

	private LevelTimes GetLevelTimes(string levelId)
	{
		if (!LevelsTimes.LevelTimes.ContainsKey(levelId)) LevelsTimes.LevelTimes.Add(levelId, new LevelTimes());
		return (LevelsTimes.LevelTimes[levelId]);
	}

	public void AddTry(string levelId)
	{
		LevelTimes levelTimes = GetLevelTimes(levelId);
		levelTimes.Tries++;
		Save();
	}

	public void AddTime(string levelId, float time)
	{
		LevelTimes levelTimes = GetLevelTimes(levelId);
		//levelTimes.Times.Add(new LevelTime(time));
		if (time < levelTimes.BestTime) levelTimes.BestTime = time;
		Save();
	}

	public void Unlock(string levelId)
	{
		GetLevelTimes(levelId).Locked = false;
		Save();
	}

	public bool HasTimes(string levelId) => GetLevelTimes(levelId).BestTime < float.MaxValue;

	public float GetBest(string levelId, float defaultValue) => Mathf.Min(GetLevelTimes(levelId).BestTime, defaultValue);

	public bool GetLocked(string levelId) => GetLevelTimes(levelId).Locked;

	private void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		using FileStream file = File.Create(GetFileName());
		bf.Serialize(file, LevelsTimes);
		file.Close();
		//SaveTxt();
	}

	private void SaveTxt()
	{
		StringBuilder content = new StringBuilder();
		foreach (string key in LevelsTimes.LevelTimes.Keys)
		{
			LevelTimes levelTimes = LevelsTimes.LevelTimes[key];
			content.AppendLine(key + "\t" + levelTimes.Tries + "\t" + levelTimes.Times.Count);
		}
		File.WriteAllText(string.Concat(Application.persistentDataPath, "/SavedTimes.txt"), content.ToString());
		//Debug.Log(string.Concat(Application.persistentDataPath, "/SavedTimes.txt"));
	}

	public void Reset()
	{
		string fileName = GetFileName();
		if (!File.Exists(fileName)) return;
		File.Delete(fileName);
		Load();
	}

	private void Load()
	{
		string fileName = GetFileName();
		if (!File.Exists(fileName))
		{
			LevelsTimes = new LevelsTimes();
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();
		using FileStream file = File.Open(fileName, FileMode.Open);
		LevelsTimes = (LevelsTimes)bf.Deserialize(file);
		file.Close();
	}

	private string GetFileName() => string.Concat(Application.persistentDataPath, SAVE_FILENAME);
}
