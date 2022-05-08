using UnityEngine;
using UnityEngine.SceneManagement;
using static EventsService;

public class GameManager : Singleton<GameManager>
{
	private static readonly string PREFAB_PATH = "Prefabs/Managers/GameManager";

	[SerializeField] private GameModel GameModel;

	[HideInInspector] public bool IsCompleteMode;
	[HideInInspector] public LevelsTimesSaver LevelsTimesSaver;
	[HideInInspector] public EventsService EventsService;
	[HideInInspector] public ParticlesService ParticlesService;
	[HideInInspector] public SoundService SoundService;
	[HideInInspector] public OptionsService OptionsService;
	[HideInInspector] public LevelService LevelService;
	[HideInInspector] public LootService LootService;
	[HideInInspector] public BackgroundsService BackgroundsService;
	[HideInInspector] public IAPService IAPService;
	[HideInInspector] public AdsService AdsService;

	public static GameObject Instantiate() => Instantiate(Resources.Load(PREFAB_PATH)) as GameObject;

	protected override void Awake()
	{
		base.Awake();

		Input.backButtonLeavesApp = true;
		LevelsTimesSaver = new LevelsTimesSaver();

		if (PlayerPrefs.GetString("Reset", string.Empty) != "1.3.1")
		{
			LevelsTimesSaver.Reset();
			PlayerPrefs.SetString("Reset", "1.3.1");
		}

		EventsService = new EventsService();

		IAPService = gameObject.AddComponent<IAPService>();
		AdsService = gameObject.AddComponent<AdsService>();

		ParticlesService = new ParticlesService(GameModel.ParticlesModel);
		SoundService = new SoundService(GameModel.SoundsModel);
		OptionsService = new OptionsService(GameModel.SoundsModel.GlobalVolume, GameModel.SoundsModel.MusicsVolume, GameModel.SoundsModel.SoundsVolume);
		LevelService = new LevelService(GameModel.LevelCatalogModel);
		LootService = new LootService(GameModel.ArcadeLootModel);
		BackgroundsService = new BackgroundsService(GameModel.BackgroundCollectionModel);

		SoundService.SetVolumes();
		OptionsService.OnOptionsChanged += SoundService.SetVolumes;
	}

	private void OnEnable() => EventsService.Register(Events.OnSceneRequested, OnSceneRequestedCallback);

	private void OnDestroy() => EventsService.UnRegister(Events.OnSceneRequested, OnSceneRequestedCallback);

	private void OnSceneRequestedCallback(EventModelArg eventArg) => LoadScene((eventArg as OnSceneRequestedEventArg).Scene);

	private void LoadScene(SceneNames sceneName)
	{
		StopAllCoroutines();
		SceneManager.LoadScene(sceneName.ToString());
	}
}

