using System.Linq;
using UnityEngine;

public class BackgroundsService
{
	private readonly BackgroundCollectionModel BackgroundCollectionModel;
	private int CurrentIndex = 0;
	private readonly int NbBackgrounds;
	private readonly Sprite[] BackgroundsShuffled;

	public BackgroundsService(BackgroundCollectionModel backgroundCollectionModel)
	{
		BackgroundCollectionModel = backgroundCollectionModel;
		CurrentIndex = -1;
		NbBackgrounds = BackgroundCollectionModel.Backgrounds.Length;
		BackgroundsShuffled = BackgroundCollectionModel.Backgrounds.OrderBy(x => Random.value).ToArray();
	}

	public Sprite GetNext()
	{
		CurrentIndex = (CurrentIndex + 1) % NbBackgrounds;
		return (BackgroundsShuffled[CurrentIndex]);
	}
}
