using TMPro;
using UnityEngine;
using static EventsService;
using static SoundService;

public class TimerController : MonoBehaviour
{
	[SerializeField] private float AlertScale = 1.1f;
	[SerializeField] private float AlertThreshold = 10.0f;
	[SerializeField] private Color AlertColor = Color.red;
	[SerializeField] private Color BestOverlapColor = Color.gray;
	[SerializeField] private float RefreshRate = 0.09f;
	[SerializeField] private TextMeshProUGUI TimerText;
	[SerializeField] private TextMeshProUGUI BestText;
	[SerializeField] private TextMeshProUGUI MaxText;
	[SerializeField] private Clips AlertSound;
	[SerializeField] private Clips FinalSound;

	private TimeWatch TimeWatch;
	private float EndTime;
	private float InitialTimer;
	private float BestTimer;
	private bool Ended = false;
	private bool Alerted = false;
	private bool BestOverlapped = false;

	private void OnEnable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.Register(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
		eventsService.Register(Events.OnBallsEnded, OnBallsEndedCallback);
		eventsService.Register(Events.OnBricksEnded, OnBricksEndedCallback);
		eventsService.Register(Events.OnLevelSetupStarted, OnLevelSetupStartedCallback);
		eventsService.Register(Events.OnTimerPaused, OnTimerPausedCallback);
		eventsService.Register(Events.OnTimerResume, OnTimerResumeCallback);
	}

	private void OnDisable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
		eventsService.UnRegister(Events.OnBallsEnded, OnBallsEndedCallback);
		eventsService.UnRegister(Events.OnBricksEnded, OnBricksEndedCallback);
		eventsService.UnRegister(Events.OnLevelSetupStarted, OnLevelSetupStartedCallback);
		eventsService.UnRegister(Events.OnTimerPaused, OnTimerPausedCallback);
		eventsService.UnRegister(Events.OnTimerResume, OnTimerResumeCallback);
	}

	public static string GetFormattedTime(float time) => (time / 1000.0f).ToString("N3");

	private void OnLevelSetupStartedCallback(EventModelArg eventArg)
	{
		OnSetupStartedEventArg onSetupStartedEventArg = eventArg as OnSetupStartedEventArg;
		InitialTimer = onSetupStartedEventArg.LevelModel.SuccessTimer;
		BestTimer = onSetupStartedEventArg.LevelModel.BestTimer;

		MaxText.text = GetFormattedTime(InitialTimer);
		BestText.text = GetFormattedTime(BestTimer);
		InitChronoText();
	}

	private void OnLevelStartedCallback(EventModelArg eventArg)
	{
		TimeWatch = TimeWatch.StartNew();
		InvokeRepeating(nameof(UpdateTimer), 0.0f, RefreshRate);
	}

	private void OnLevelEndedCallback(EventModelArg eventArg)
	{
		CancelInvoke(nameof(FlashTimer));
		CancelInvoke(nameof(UpdateTimer));
	}

	private void StopStopwatch()
	{
		EndTime = Time.time;
		Ended = true;
		CancelInvoke(nameof(UpdateTimer));
	}

	private void OnBallsEndedCallback(EventModelArg eventArg) => StopStopwatch();

	private void OnTimerPausedCallback(EventModelArg eventArg)
	{
		TimeWatch.Stop();
		Color originalColor = TimerText.color;
		TimerText.ColorTo(Color.green, 0.5f, Tweening.QuintInOut);
	}

	private void OnTimerResumeCallback(EventModelArg eventArg)
	{
		TimerText.ColorTo(Color.white, 0.5f, Tweening.QuintInOut);
		if (!Ended) TimeWatch.Start();
	}

	private void OnBricksEndedCallback(EventModelArg eventArg)
{
		StopStopwatch();
		UpdateChronoText();
		if (TimeWatch.Elapsed <= BestTimer)
		{
			BestText.text = GetFormattedTime(TimeWatch.Elapsed);
			TimerText.color = new Color(1.0f, 0.65f, 0.0f, 1.0f);
		}
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnTimerEnded, new OnTimerEndedEventArg() { Timer = Mathf.Clamp(TimeWatch.Elapsed, 0.0f, InitialTimer) });
	}

	private void InitChronoText() => TimerText.text = GetFormattedTime(0.0f);

	private void UpdateChronoText() => TimerText.text = GetFormattedTime(Mathf.Clamp(TimeWatch.Elapsed, 0.0f, InitialTimer));

	private void FlashTimer()
	{
		GameManager.Instance.GetService<SoundService>().Play(AlertSound);
		Vector3 initialScale = TimerText.transform.localScale;
		TimerText.transform.localScale = new Vector3(initialScale.x * AlertScale, initialScale.y * AlertScale, 1.0f);
		TimerText.transform.ZoomTo(initialScale, 0.2f, Tweening.QuintIn);
	}

	private void UpdateTimer()
	{
		UpdateChronoText();

		if (TimeWatch.Elapsed >= InitialTimer)
		{
			StopStopwatch();
			GameManager.Instance.GetService<SoundService>().Play(FinalSound);
			GameManager.Instance.GetService<EventsService>().Raise(Events.OnTimerEnded, new OnTimerEndedEventArg() { Timer = 0.0f });
			TimerText.text = GetFormattedTime(InitialTimer);
			return;
		}

		if (!Alerted && (TimeWatch.Elapsed >= (InitialTimer - AlertThreshold * 1000.0f)))
		{
			Alerted = true;
			TimerText.ColorTo(AlertColor, 0.5f, Tweening.QuintInOut);
			InvokeRepeating(nameof(FlashTimer), 0.0f, 1.0f);
		}

		if (!BestOverlapped && (TimeWatch.Elapsed >= BestTimer))
		{
			BestOverlapped = true;
			BestText.ColorTo(BestOverlapColor, 0.5f, Tweening.QuintInOut);
		}
	}
}
