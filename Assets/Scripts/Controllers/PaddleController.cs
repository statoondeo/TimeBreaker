using UnityEngine;
using UnityEngine.InputSystem;
using static EventsService;
using static SoundService;

public class PaddleController : MonoBehaviour
{
	private Camera MainCamera;
	private bool Activated = false;
	private Vector3 InitialScale;
	private Vector3 TargetScale;
	private PaddlePhysicController PaddlePhysicController;

	[SerializeField] private float Bounds;
	[SerializeField] private Particles CollisionParticles;
	[SerializeField] private Particles DestroyParticles;
	[SerializeField] private Clips CollisionSound;
	[SerializeField] private Clips ExplosionSound;
	[SerializeField] private float ZoomFactor = 0.25f;

	[SerializeField] private InputAction TouchPosition;

	private void Awake()
	{
		MainCamera = Camera.main;
		InitialScale = transform.localScale;
		TargetScale = new Vector3(InitialScale.x * ZoomFactor, InitialScale.y * ZoomFactor, 1.0f);
		PaddlePhysicController = GetComponent<PaddlePhysicController>();
	}

	private void OnEnable()
	{
		GameManager.Instance.EventsService.Register(Events.OnLevelStarted, OnLevelStartedCallback);
		GameManager.Instance.EventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
		TouchPosition.Enable();
		TouchPosition.performed += OnPositionChanged;
	}

	private void OnDisable()
	{
		TouchPosition.performed -= OnPositionChanged;
		TouchPosition.Disable();
		GameManager.Instance.EventsService.UnRegister(Events.OnLevelStarted, OnLevelStartedCallback);
		GameManager.Instance.EventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnPositionChanged(InputAction.CallbackContext context)
{
		if (!Activated) return;

		Vector2 position = context.ReadValue<Vector2>();
		Vector3 newPosition = MainCamera.ScreenToWorldPoint(new Vector3(position.x, position.y));
		transform.position = new Vector3(Mathf.Clamp(newPosition.x, -Bounds, Bounds), transform.position.y, transform.position.z);
	}

	private void Start() => GameManager.Instance.EventsService.Raise(Events.OnPaddlePopped, new OnPaddlePoppedEventArg() { PaddleController = this });

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!Activated) return;

		GameManager.Instance.SoundService.Play(CollisionSound);
		GameManager.Instance.ParticlesService.Get(CollisionParticles, transform.position).Play();

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
		GameManager.Instance.SoundService.Play(ExplosionSound);
		GameManager.Instance.ParticlesService.Get(DestroyParticles, transform.position).Play();
		gameObject.SetActive(false);
	}
}
