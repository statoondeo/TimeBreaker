using UnityEngine;
using static SoundService;

//[RequireComponent(typeof(BlinkerController))]
[DisallowMultipleComponent()]
public class IndestructibleBrickController : MonoBehaviour
{
	[SerializeField] private Particles CollisionParticles;
	[SerializeField] private Clips CollisionSound;
	[SerializeField] private float ZoomFactor = 2f;
	[SerializeField] private float ZoomDuration = 1.5f;

	private BlinkerController BlinkerController;

	protected virtual void Awake() => BlinkerController = GetComponent<BlinkerController>();

	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		GameManager.Instance.SoundService.Play(CollisionSound);
		StartCoroutine(BlinkerController.BlinkRoutine());
		ParticleSystem particles = GameManager.Instance.ParticlesService.Get(CollisionParticles, transform.position);
		particles.transform.localScale *= collision.GetContact(0).normalImpulse + Random.value;
		particles.Play();
	}
}

