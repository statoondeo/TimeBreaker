using UnityEngine;
using UnityEngine.SceneManagement;
using static EventsService;

public class GameManager : Singleton<GameManager>
{
	private static readonly string PREFAB_PATH = "Prefabs/Managers/GameManager";

	[SerializeField] private GameModel GameModel;

	[HideInInspector] public bool IsCompleteMode;

	private ServiceLocator ServiceLocator;

	public static GameObject Instantiate() => Instantiate(Resources.Load(PREFAB_PATH)) as GameObject;

	protected override void Awake()
	{
		base.Awake();

		ServiceLocator = new ServiceLocator();

		ServiceLocator.Register(new LevelsTimesSaver());
		ServiceLocator.Register(new EventsService());
		ServiceLocator.Register(gameObject.AddComponent<IAPService>());
		ServiceLocator.Register(gameObject.AddComponent<AdsService>());
		ServiceLocator.Register(gameObject.AddComponent<InputService>());
		ServiceLocator.Register(new ParticlesService(GameModel.ParticlesModel));
		SoundService soundService = ServiceLocator.Register(new SoundService(GameModel.SoundsModel));
		OptionsService optionsService = ServiceLocator.Register(new OptionsService(GameModel.SoundsModel.GlobalVolume, GameModel.SoundsModel.MusicsVolume, GameModel.SoundsModel.SoundsVolume));
		ServiceLocator.Register(new LevelService(GameModel.LevelCatalogModel));
		ServiceLocator.Register(new LootService(GameModel.ArcadeLootModel));
		ServiceLocator.Register(new BackgroundsService(GameModel.BackgroundCollectionModel));

		soundService.SetVolumes();
		optionsService.OnOptionsChanged += soundService.SetVolumes;
	}

	public T GetService<T>() where T : class => ServiceLocator.Get<T>();

	private void OnEnable() => GetService<EventsService>().Register(Events.OnSceneRequested, OnSceneRequestedCallback);

	private void OnDestroy() => GetService<EventsService>().UnRegister(Events.OnSceneRequested, OnSceneRequestedCallback);

	private void OnSceneRequestedCallback(EventModelArg eventArg) => LoadScene((eventArg as OnSceneRequestedEventArg).Scene);

	private void LoadScene(SceneNames sceneName)
	{
		StopAllCoroutines();
		SceneManager.LoadScene(sceneName.ToString());
	}
}

