using System.Linq;

public class LevelService : IService
{
    private readonly LevelCatalogModel LevelCatalogModel;

    public int CurrentLevelIndex
	{
        get => LevelCatalogModel.CurrentLevelIndex;
        set => LevelCatalogModel.CurrentLevelIndex = value;
    }

    public LevelModel[] Levels => LevelCatalogModel.Levels;

    public LevelService(LevelCatalogModel levelCatalogModel) => LevelCatalogModel = levelCatalogModel;
    
    public LevelModel GetCurrentLevelModel() => LevelCatalogModel.Levels[LevelCatalogModel.CurrentLevelIndex];

    public LevelModel GetLevelModel(string id) => LevelCatalogModel.Levels.First(x => x.Id.Equals(id));

    public void GotoNextLevel() => LevelCatalogModel.CurrentLevelIndex = (LevelCatalogModel.CurrentLevelIndex + 1) % LevelCatalogModel.Levels.Length;
}