using UnityEngine;
using static EventsService;
using static SoundService;

public class BlinkingShieldController : MonoBehaviour
{
	[SerializeField] private GameObject ShieldGameObject;
	[SerializeField] private float Duration = .5f;
	[SerializeField] private Clips ShieldIn;
	[SerializeField] private Clips ShieldOut;
	[SerializeField] private BadgeController BadgeController;

	private Vector3 TargetScale;

	private void Awake()
	{
		BadgeController.OnReady += BadgeController_OnReady;
		BadgeController.Init();
		ShieldGameObject.transform.localScale = Vector3.zero;
		TargetScale = Vector3.one;
	}

	private void OnEnable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.Register(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnDisable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnLevelStartedCallback(EventModelArg eventArg) => BadgeController.StartBadgeTimer();

	private void OnLevelEndedCallback(EventModelArg eventArg) => BadgeController.OnReady -= BadgeController_OnReady;

	private void BadgeController_OnReady()
	{
		BadgeController.StartBadgeTimer();
		if (TargetScale == Vector3.one)
		{
			GameManager.Instance.GetService<SoundService>().Play(ShieldOut);
			ShieldGameObject.transform.ZoomTo(TargetScale, Duration, Tweening.ElasticOut);
			TargetScale = Vector3.zero;
		}
		else
		{
			GameManager.Instance.GetService<SoundService>().Play(ShieldIn);
			ShieldGameObject.transform.ZoomTo(TargetScale, Duration, Tweening.ElasticIn);
			TargetScale = Vector3.one;
		}
	}
}
