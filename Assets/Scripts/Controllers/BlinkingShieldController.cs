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
		GameManager.Instance.EventsService.Register(Events.OnLevelStarted, OnLevelStartedCallback);
		GameManager.Instance.EventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnDisable()
	{
		GameManager.Instance.EventsService.UnRegister(Events.OnLevelStarted, OnLevelStartedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnLevelStartedCallback(EventModelArg eventArg) => BadgeController.StartBadgeTimer();

	private void OnLevelEndedCallback(EventModelArg eventArg) => BadgeController.OnReady -= BadgeController_OnReady;

	private void BadgeController_OnReady()
	{
		BadgeController.StartBadgeTimer();
		if (TargetScale == Vector3.one)
		{
			GameManager.Instance.SoundService.Play(ShieldOut);
			ShieldGameObject.transform.ZoomTo(TargetScale, Duration, Tweening.ElasticOut);
			TargetScale = Vector3.zero;
		}
		else
		{
			GameManager.Instance.SoundService.Play(ShieldIn);
			ShieldGameObject.transform.ZoomTo(TargetScale, Duration, Tweening.ElasticIn);
			TargetScale = Vector3.one;
		}
	}
}
