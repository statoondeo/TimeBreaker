using System.Linq;
using UnityEngine;

public class LevelService : MonoBehaviour, IService
{
    public LevelCatalogModel LevelCatalogModel;
    private LevelCatalogModel CurrentCatalogModel;

    private void Start()
	{
#if UNITY_WEBGL
        CurrentCatalogModel = ScriptableObject.CreateInstance<LevelCatalogModel>();
        int maxNbLevel = 4;
        CurrentCatalogModel.Levels = new LevelModel[maxNbLevel];
		for (int i = 0; i < maxNbLevel; i++)
		{
            CurrentCatalogModel.Levels[i] = LevelCatalogModel.Levels[i];
        }
#else
        CurrentCatalogModel = LevelCatalogModel;
#endif
    }

    public int CurrentLevelIndex
	{
        get => CurrentCatalogModel.CurrentLevelIndex;
        set => CurrentCatalogModel.CurrentLevelIndex = value;
    }

    public LevelModel[] Levels => CurrentCatalogModel.Levels;

    public LevelService(LevelCatalogModel levelCatalogModel) => CurrentCatalogModel = levelCatalogModel;
    
    public LevelModel GetCurrentLevelModel() => CurrentCatalogModel.Levels[CurrentCatalogModel.CurrentLevelIndex];

    public LevelModel GetLevelModel(string id) => CurrentCatalogModel.Levels.First(x => x.Id.Equals(id));

    public void GotoNextLevel() => CurrentCatalogModel.CurrentLevelIndex = (CurrentCatalogModel.CurrentLevelIndex + 1) % CurrentCatalogModel.Levels.Length;
}