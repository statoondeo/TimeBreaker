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
		GameManager.Instance.ParticlesService.Reset();
		GameManager.Instance.LootService.Init();

		CurrentLevelModel = GameManager.Instance.LevelService.GetCurrentLevelModel();
		LevelsTimesSaver = GameManager.Instance.LevelsTimesSaver;
		LevelsTimesSaver.Unlock(CurrentLevelModel.Id);
		CurrentLevelModel.BestTimer = LevelsTimesSaver.GetBest(CurrentLevelModel.Id, CurrentLevelModel.SuccessTimer);
		LevelBrickCount = 0;
		LevelBallCount = 0;
		SetupCurves = new List<CurveFollower>();
	}

	private void OnEnable()
	{
		GameManager.Instance.EventsService.Register(Events.OnTimerEnded, OnTimerEndedCallback);
		GameManager.Instance.EventsService.Register(Events.OnBrickPopped, OnBrickPoppedCallback);
		GameManager.Instance.EventsService.Register(Events.OnBrickKilled, OnBrickKilledCallback);
		GameManager.Instance.EventsService.Register(Events.OnBallPopped, OnBallPoppedCallback);
		GameManager.Instance.EventsService.Register(Events.OnBallKilled, OnBallKilledCallback);
	}

	private void OnDisable()
	{
		GameManager.Instance.EventsService.UnRegister(Events.OnTimerEnded, OnTimerEndedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBrickPopped, OnBrickPoppedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBrickKilled, OnBrickKilledCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBallPopped, OnBallPoppedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBallKilled, OnBallKilledCallback);
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
		GameManager.Instance.EventsService.UnRegister(Events.OnTimerEnded, OnTimerEndedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBrickPopped, OnBrickPoppedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBrickKilled, OnBrickKilledCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBallPopped, OnBallPoppedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnBallKilled, OnBallKilledCallback);

		float score = (eventArg as OnTimerEndedEventArg).Timer;
		if (score == 0.0f)
		{
			GameManager.Instance.EventsService.Raise(Events.OnLevelEnded);
			ChangeFinalText("TimeoutTitle");
		}
		else
		{
			LevelsTimesSaver.AddTime(CurrentLevelModel.Id, score);
			GameManager.Instance.LevelService.GotoNextLevel();
			LevelsTimesSaver.Unlock(GameManager.Instance.LevelService.GetCurrentLevelModel().Id);
			GameManager.Instance.EventsService.Raise(Events.OnLevelEnded, new OnLevelEndedEventArg()
			{
				Timer = CurrentLevelModel.SuccessTimer,
				Best = LevelsTimesSaver.GetBest(CurrentLevelModel.Id, CurrentLevelModel.SuccessTimer),
				Score = score
			});
		}
		GameManager.Instance.SoundService.StopMusic();
		Cursor.visible = true;
	}

	private void OnBallPoppedCallback(EventModelArg eventArg) => LevelBallCount++;

	private void OnBallKilledCallback(EventModelArg eventArg)
	{
		LevelBallCount--;
		if (LevelBallCount > 0) return;

		GameManager.Instance.EventsService.UnRegister(Events.OnBallKilled, OnBallKilledCallback);
		GameManager.Instance.SoundService.StopMusic();
		Cursor.visible = true;
		ChangeFinalText("DefeatTitle");
		GameManager.Instance.EventsService.Raise(Events.OnBallsEnded);
		GameManager.Instance.EventsService.Raise(Events.OnLevelEnded);
	}

	private void OnBrickPoppedCallback(EventModelArg eventArg) => LevelBrickCount++;

	private void OnBrickKilledCallback(EventModelArg eventArg)
	{
		LevelBrickCount--;
		if (LevelBrickCount <= 0) GameManager.Instance.EventsService.Raise(Events.OnBricksEnded);
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
		GameManager.Instance.EventsService.Raise(Events.OnLevelSetupStarted, new OnSetupStartedEventArg() { LevelModel = CurrentLevelModel });
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

		GameManager.Instance.EventsService.Raise(Events.OnLevelSetupEnded);
		StartLevel();
	}
	private void StartLevel()
	{
		LevelsTimesSaver.AddTry(CurrentLevelModel.Id);
		GameManager.Instance.SoundService.Play(GameManager.Instance.SoundService.GetNextMusic());
		GameManager.Instance.EventsService.Raise(Events.OnLevelStarted, new OnLevelStartedEventArg() { Timer = CurrentLevelModel.SuccessTimer });
	}

	public void OnMenuClick() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });

	public void OnSelectionClick() => StartCoroutine(GotoNextScene(GotoSelection));

	public void OnRetryClick() => StartCoroutine(GotoNextScene(GotoGameplay));

	public void OnShopClick() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Shop });

	public void OnContinueClick() => StartCoroutine(GotoNextScene(GotoGameplay));

	private IEnumerator GotoNextScene(Action callback)
	{
		yield return (new WaitForSeconds(.1f));
		GameManager.Instance.AdsService.ShowAd(callback);
	}

	private void GotoGameplay() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Gameplay });
	private void GotoSelection() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Selection });
}
