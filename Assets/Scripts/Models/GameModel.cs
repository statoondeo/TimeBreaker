using UnityEngine;

[CreateAssetMenu(menuName = "New Game Model", fileName = "New Game Model")]
public class GameModel : ScriptableObject
{
	public bool IsComplete;
	public BackgroundCollectionModel BackgroundCollectionModel;
	public LevelCatalogModel LevelCatalogModel;
	public SoundsModel SoundsModel;
	public LootModel ArcadeLootModel;
	public ParticlesModel ParticlesModel;
}
