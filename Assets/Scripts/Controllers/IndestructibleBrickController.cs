using UnityEngine;
using static EventsService;
using static SoundService;

//[RequireComponent(typeof(BlinkerController))]
[DisallowMultipleComponent()]
public class IndestructibleBrickController : MonoBehaviour
{
	[SerializeField] private Particles CollisionParticles;
	[SerializeField] private Clips CollisionSound;

	private BlinkerController BlinkerController;
	protected bool Activated = false;

	protected virtual void Awake() => BlinkerController = GetComponent<BlinkerController>();

	private void OnEnable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.Register(Events.OnLevelStarted, OnLevelSartedCallback);
		eventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnDisable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnLevelStarted, OnLevelSartedCallback);
		eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	protected virtual void OnLevelSartedCallback(EventModelArg eventArg) => Activated = true;

	protected virtual void OnLevelEndedCallback(EventModelArg eventArg) => Activated = false;

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (!Activated) return;

		GameManager.Instance.GetService<SoundService>().Play(CollisionSound);
		StartCoroutine(BlinkerController.BlinkRoutine());
		ParticleSystem particles = GameManager.Instance.GetService<ParticlesService>().Get(CollisionParticles, transform.position);
		particles.transform.localScale *= collision.GetContact(0).normalImpulse + Random.value;
		particles.Play();
	}
}

