using UnityEngine;
using static EventsService;

public class CameraShakeController : MonoBehaviour
{
	[SerializeField] private float Duration = 0.25f;
	[SerializeField] private float Intensity = .15f;
	[SerializeField] private GameObject Background;

	private Vector3 CameraInitialPosition;
	private Vector3 BackgroundInitialPosition;
	private Coroutine ShakingRoutine;

	private void Awake()
	{		
		CameraInitialPosition = transform.position;
		BackgroundInitialPosition = Background.transform.position;
	}

	private void OnEnable() => GameManager.Instance.EventsService.Register(Events.OnBallCollided, OnBallCollidedCallback);

	private void OnDisable() => GameManager.Instance.EventsService.UnRegister(Events.OnBallCollided, OnBallCollidedCallback);

	private void OnBallCollidedCallback(EventModelArg eventArg)
	{
		if (null != ShakingRoutine) StopAllCoroutines();

		Collision2D collision = (eventArg as OnBallCollidedEventArg).Collision;
		Vector2 impulse = collision.GetContact(0).normal;
		Vector3 newPositionDelta = Intensity * (new Vector3(impulse.x, impulse.y));

		transform.position += newPositionDelta;
		ShakingRoutine = transform.MoveTo(CameraInitialPosition, Duration, Tweening.QuintOut);

		Background.transform.position += newPositionDelta;
		Background.transform.MoveTo(BackgroundInitialPosition, Duration, Tweening.QuintOut);
	}
}
