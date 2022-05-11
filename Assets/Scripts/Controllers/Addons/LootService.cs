using UnityEngine;

public class LootService : IService
{
	private readonly LootModel LootModel;
	private int NbLoots;
	private int CurrentLoot;
	private GameObject[] Loots;

	public LootService(LootModel lootModel) => LootModel = lootModel;

	public void Init()
	{
		Loots = new GameObject[LootModel.PreLoad];
		NbLoots = LootModel.Loots.Length;

		float totalLootRate = 0.0f;
		for (int i = 0; i < NbLoots; i++) totalLootRate += LootModel.Loots[i].LootRate;

		float cumulativeLootRate = 0.0f;
		for (int i = 0; i < NbLoots; i++)
		{
			cumulativeLootRate += LootModel.Loots[i].LootRate;
			LootModel.Loots[i].LootThreshold = cumulativeLootRate / totalLootRate;
		}

		float lootValue;
		for (int i = 0; i < LootModel.PreLoad; i++)
		{
			lootValue = Random.value;
			for (int j = 0; j < NbLoots; j++)
			{
				if (lootValue >= LootModel.Loots[j].LootThreshold) continue;

				Loots[i] = LootModel.Loots[j].LootPrefab;
				break;
			}
		}
		CurrentLoot = -1;
	}

	public GameObject GetNextLoot()
	{
		CurrentLoot = (CurrentLoot + 1) % LootModel.PreLoad;
		return (Loots[CurrentLoot]);
	}
}

