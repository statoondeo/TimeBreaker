using UnityEngine;
using UnityEngine.SceneManagement;
using static EventsService;

public class GameManager : Singleton<GameManager>
{
	private static readonly string PREFAB_PATH = "Prefabs/Managers/GameManager";

	[SerializeField] private GameModel DemoGameModel;
	[SerializeField] private GameModel CompleteGameModel;
	[SerializeField] private bool StartComplete;

	[HideInInspector] public LevelsTimesSaver LevelsTimesSaver;
	[HideInInspector] public EventsService EventsService;
	[HideInInspector] public ParticlesService ParticlesService;
	[HideInInspector] public SoundService SoundService;
	[HideInInspector] public OptionsService OptionsService;
	[HideInInspector] public LevelService LevelService;
	[HideInInspector] public LootService LootService;
	[HideInInspector] public BackgroundsService BackgroundsService;
	[HideInInspector] public IAPService IAPService;

	private GameModel CurrentGameModel;

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

		ChangeModel(StartComplete ? CompleteGameModel : DemoGameModel);
	}

	private void OnEnable() => EventsService.Register(Events.OnSceneRequested, OnSceneRequestedCallback);

	private void OnDestroy() => EventsService.UnRegister(Events.OnSceneRequested, OnSceneRequestedCallback);

	public void SwitchToDemoVersion() => ChangeModel(DemoGameModel);

	public void SwitchToCompleteVersion() => ChangeModel(CompleteGameModel);

	private void ChangeModel(GameModel gameModel)
	{
		if (null != OptionsService)
		{
			OptionsService.OnOptionsChanged -= SoundService.SetVolumes;
			SoundService.StopMusic();
		}

		CurrentGameModel = gameModel;

		ParticlesService = new ParticlesService(CurrentGameModel.ParticlesModel);
		SoundService = new SoundService(CurrentGameModel.SoundsModel);
		OptionsService = new OptionsService(CurrentGameModel.SoundsModel.GlobalVolume, CurrentGameModel.SoundsModel.MusicsVolume, CurrentGameModel.SoundsModel.SoundsVolume);
		LevelService = new LevelService(CurrentGameModel.LevelCatalogModel);
		LootService = new LootService(CurrentGameModel.ArcadeLootModel);
		BackgroundsService = new BackgroundsService(CurrentGameModel.BackgroundCollectionModel);

		SoundService.SetVolumes();
		OptionsService.OnOptionsChanged += SoundService.SetVolumes;
	}

	public bool IsCompleteMode => CurrentGameModel.IsComplete;

	private void OnSceneRequestedCallback(EventModelArg eventArg) => LoadScene((eventArg as OnSceneRequestedEventArg).Scene);

	private void LoadScene(SceneNames sceneName)
	{
		StopAllCoroutines();
		SceneManager.LoadScene(sceneName.ToString());
	}
}

