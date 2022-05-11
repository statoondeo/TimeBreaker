using UnityEngine;
using static EventsService;
using static SoundService;

public class PaddleController : MonoBehaviour
{
	private Camera MainCamera;
	private bool Activated = false;
	private Vector3 InitialScale;
	private Vector3 TargetScale;
	private PaddlePhysicController PaddlePhysicController;
	private float Sensitivity;

	[SerializeField] private float Bounds;
	[SerializeField] private Particles CollisionParticles;
	[SerializeField] private Particles DestroyParticles;
	[SerializeField] private Clips CollisionSound;
	[SerializeField] private Clips ExplosionSound;
	[SerializeField] private float ZoomFactor = 0.25f;

	private void Awake()
	{
		MainCamera = Camera.main;
		InitialScale = transform.localScale;
		TargetScale = new Vector3(InitialScale.x * ZoomFactor, InitialScale.y * ZoomFactor, 1.0f);
		PaddlePhysicController = GetComponent<PaddlePhysicController>();
		Sensitivity = GameManager.Instance.GetService<OptionsService>().SensitivityLevel;
	}

	private void OnEnable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.Register(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
		GameManager.Instance.GetService<InputService>().OnPositionChanged += OnPositionChanged;
	}

	private void OnDisable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		GameManager.Instance.GetService<InputService>().OnPositionChanged -= OnPositionChanged;
		eventsService.UnRegister(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnPositionChanged(Vector2 position)
{
		if (!Activated) return;

		Vector3 newPosition = MainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y));
		transform.position = new Vector3(Mathf.Clamp(newPosition.x * Sensitivity, -Bounds, Bounds), transform.position.y, transform.position.z);
	}

	private void Start() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnPaddlePopped, new OnPaddlePoppedEventArg() { PaddleController = this });

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!Activated) return;

		GameManager.Instance.GetService<SoundService>().Play(CollisionSound);
		GameManager.Instance.GetService<ParticlesService>().Get(CollisionParticles, transform.position).Play();

		transform.localScale = TargetScale;
		transform.ZoomTo(InitialScale, 0.5f, Tweening.ElasticOut);
	}

	private void OnLevelStartedCallback(EventModelArg eventArg)
	{
		Activated = true;
		PaddlePhysicController.Activated = Activated;
	}

	private void OnLevelEndedCallback(EventModelArg eventArg)
	{
		GameManager.Instance.GetService<SoundService>().Play(ExplosionSound);
		GameManager.Instance.GetService<ParticlesService>().Get(DestroyParticles, transform.position).Play();
		gameObject.SetActive(false);
	}
}
