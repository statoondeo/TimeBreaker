using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using static EventsService;

public class LevelController : MonoBehaviour
{
	[SerializeField] private LocalizeStringEvent EndReason;
	[SerializeField] private Transform PhysicObjectsContainer;

	private List<CurveFollower> SetupCurves;
	private int SetupDoneCounter;
	private int LevelBrickCount;
	private int LevelBallCount;
	private LevelModel CurrentLevelModel;
	private LevelsTimesSaver LevelsTimesSaver;

	private void Awake()
	{
		GameManager.Instance.GetService<ParticlesService>().Reset();
		GameManager.Instance.GetService<LootService>().Init();

		CurrentLevelModel = GameManager.Instance.GetService<LevelService>().GetCurrentLevelModel();
		LevelsTimesSaver = GameManager.Instance.GetService<LevelsTimesSaver>();
		LevelsTimesSaver.Unlock(CurrentLevelModel.Id);
		CurrentLevelModel.BestTimer = LevelsTimesSaver.GetBest(CurrentLevelModel.Id, CurrentLevelModel.SuccessTimer);
		LevelBrickCount = 0;
		LevelBallCount = 0;
		SetupCurves = new List<CurveFollower>();
	}

	private void OnEnable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.Register(Events.OnTimerEnded, OnTimerEndedCallback);
		eventsService.Register(Events.OnBrickPopped, OnBrickPoppedCallback);
		eventsService.Register(Events.OnBrickKilled, OnBrickKilledCallback);
		eventsService.Register(Events.OnBallPopped, OnBallPoppedCallback);
		eventsService.Register(Events.OnBallKilled, OnBallKilledCallback);
	}

	private void OnDisable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnTimerEnded, OnTimerEndedCallback);
		eventsService.UnRegister(Events.OnBrickPopped, OnBrickPoppedCallback);
		eventsService.UnRegister(Events.OnBrickKilled, OnBrickKilledCallback);
		eventsService.UnRegister(Events.OnBallPopped, OnBallPoppedCallback);
		eventsService.UnRegister(Events.OnBallKilled, OnBallKilledCallback);
	}

	private void PerformBrickModel(BrickInLevelModel brickInLevelModel, bool rootObject = false)
	{
		GameObject newGameObject = rootObject ? Instantiate(Resources.Load<GameObject>(brickInLevelModel.BrickPrefab)) : Instantiate(Resources.Load<GameObject>(brickInLevelModel.BrickPrefab), PhysicObjectsContainer);

		AddSetupCurve(newGameObject, brickInLevelModel.SetupCurve);
	}

	private void ChangeFinalText(string entry)
	{
		EndReason.SetEntry(entry);
		EndReason.RefreshString();
	}

	private void OnTimerEndedCallback(EventModelArg eventArg)
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnTimerEnded, OnTimerEndedCallback);
		eventsService.UnRegister(Events.OnBrickPopped, OnBrickPoppedCallback);
		eventsService.UnRegister(Events.OnBrickKilled, OnBrickKilledCallback);
		eventsService.UnRegister(Events.OnBallPopped, OnBallPoppedCallback);
		eventsService.UnRegister(Events.OnBallKilled, OnBallKilledCallback);

		float score = (eventArg as OnTimerEndedEventArg).Timer;
		if (score == 0.0f)
		{
			eventsService.Raise(Events.OnLevelEnded);
			ChangeFinalText("TimeoutTitle");
		}
		else
		{
			LevelsTimesSaver.AddTime(CurrentLevelModel.Id, score);
			LevelService levelService = GameManager.Instance.GetService<LevelService>();
			levelService.GotoNextLevel();
			LevelsTimesSaver.Unlock(levelService.GetCurrentLevelModel().Id);
			eventsService.Raise(Events.OnLevelEnded, new OnLevelEndedEventArg()
			{
				Timer = CurrentLevelModel.SuccessTimer,
				Best = LevelsTimesSaver.GetBest(CurrentLevelModel.Id, CurrentLevelModel.SuccessTimer),
				Score = score
			});
		}
		GameManager.Instance.GetService<SoundService>().StopMusic();
		Cursor.visible = true;
	}

	private void OnBallPoppedCallback(EventModelArg eventArg) => LevelBallCount++;

	private void OnBallKilledCallback(EventModelArg eventArg)
	{
		LevelBallCount--;
		if (LevelBallCount > 0) return;

		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnBallKilled, OnBallKilledCallback);
		GameManager.Instance.GetService<SoundService>().StopMusic();
		Cursor.visible = true;
		ChangeFinalText("DefeatTitle");
		eventsService.Raise(Events.OnBallsEnded);
		eventsService.Raise(Events.OnLevelEnded);
	}

	private void OnBrickPoppedCallback(EventModelArg eventArg) => LevelBrickCount++;

	private void OnBrickKilledCallback(EventModelArg eventArg)
	{
		LevelBrickCount--;
		if (LevelBrickCount <= 0) GameManager.Instance.GetService<EventsService>().Raise(Events.OnBricksEnded);
	}

	private void AddSetupCurve(GameObject gameObject, SetupCurveParameter setupCurveParameter)
	{
		if (!setupCurveParameter.HasParameter) return;

		CurveFollower curveFollower = CurveFollower.AddCurveFollower(gameObject, setupCurveParameter);
		curveFollower.OnDoneCommand = new CompositeCommand(new List<ICommand>() { new CallbackCommand(SetupDone), new DestroyComponentCommand(curveFollower) });
		SetupCurves.Add(curveFollower);
	}

	private void Start()
	{
		Cursor.visible = false;
		PerformBrickModel(CurrentLevelModel.BallModel, true);
		PerformBrickModel(CurrentLevelModel.PaddleModel, true);
		for (int i = 0, nbItems = CurrentLevelModel.BrickModels.Count; i < nbItems; i++) PerformBrickModel(CurrentLevelModel.BrickModels[i]);
		StartSetup();
	}

	private void StartSetup()
	{
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnLevelSetupStarted, new OnSetupStartedEventArg() { LevelModel = CurrentLevelModel });
		SetupDoneCounter = 0;
		CurveFollower curveFollower;
		for (int i = 0, nbItems = SetupCurves.Count; i < nbItems; i++)
		{
			curveFollower = SetupCurves[i];
			curveFollower.enabled = true;
			curveFollower.StartCurve();
		}
	}

	private void SetupDone()
	{
		SetupDoneCounter++;
		if (SetupDoneCounter < SetupCurves.Count) return;

		GameManager.Instance.GetService<EventsService>().Raise(Events.OnLevelSetupEnded);
		StartLevel();
	}
	private void StartLevel()
	{
		LevelsTimesSaver.AddTry(CurrentLevelModel.Id);
		GameManager.Instance.GetService<SoundService>().Play(GameManager.Instance.GetService<SoundService>().GetNextMusic());
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnLevelStarted, new OnLevelStartedEventArg() { Timer = CurrentLevelModel.SuccessTimer });
	}

	public void OnMenuClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });

	public void OnSelectionClick() => StartCoroutine(GotoNextScene(GotoSelection));

	public void OnRetryClick() => StartCoroutine(GotoNextScene(GotoGameplay));

	public void OnShopClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Shop });

	public void OnContinueClick() => StartCoroutine(GotoNextScene(GotoGameplay));

	private IEnumerator GotoNextScene(Action callback)
	{
		yield return (new WaitForSeconds(.1f));
		GameManager.Instance.GetService<AdsService>().ShowAd(callback);
	}

	private void GotoGameplay() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Gameplay });

	private void GotoSelection() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Selection });
}
