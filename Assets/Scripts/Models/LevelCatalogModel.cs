using UnityEngine;

[CreateAssetMenu(menuName = "New Level Catalog", fileName = "New Level Catalog")]
public class LevelCatalogModel : ScriptableObject
{
    public LevelModel[] Levels;
    public int CurrentLevelIndex;
}
