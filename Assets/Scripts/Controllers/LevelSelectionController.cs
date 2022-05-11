using UnityEngine;
using static EventsService;

public class LevelSelectionController : MonoBehaviour
{
	[SerializeField] private RectTransform LevelItemsContainer;
	[SerializeField] private GameObject LevelItemPrefab;

	[SerializeField] private GameObject BuyButton;

	private void Awake() => BuyButton.SetActive(GameManager.Instance.GetService<IAPService>().IsBuyButtonActivated);

	private void Start() => Init();

	private void Clear()
	{
		for (int i = 0, nbItems = LevelItemsContainer.transform.childCount; i < nbItems; i++) Destroy(LevelItemsContainer.transform.GetChild(i).gameObject);
	}

	private void Init()
	{
		Clear();
		LevelService levelService = GameManager.Instance.GetService<LevelService>();
		for (int i = 0, nbItems = levelService.Levels.Length; i < nbItems; i++)
			Instantiate(LevelItemPrefab, LevelItemsContainer.transform).GetComponent<LevelItemController>().Init(i, levelService.Levels[i]);
	}

	public void OnBackClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });
	
	public void OnShopClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Shop });
}
