using UnityEngine;
using static SoundService;

//[RequireComponent(typeof(IndestructibleBrickController))]
public class BonusBrickCollisionController : MonoBehaviour
{
	[SerializeField] private GameObject BallPrefab;
	[SerializeField] private BadgeController BadgeController;
	[SerializeField] private Particles CollisionParticles;
	[SerializeField] private Clips BonusSound;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!BadgeController.CanStart) return;

		GameManager.Instance.SoundService.Play(BonusSound);
		BadgeController.StartBadgeTimer();

		Rigidbody2D rigidbody2D =Instantiate(GameManager.Instance.LootService.GetNextLoot(), transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
		rigidbody2D.velocity = 2.0f * collision.contacts[0].normalImpulse * collision.contacts[0].normal;

		ParticleSystem particles = GameManager.Instance.ParticlesService.Get(CollisionParticles, transform.position, Quaternion.FromToRotation(transform.up, -collision.contacts[0].normal));
		ParticleSystem.MainModule mainModule = particles.main;
		mainModule.startSpeedMultiplier = collision.contacts[0].normalImpulse;
		particles.Play();
	}
}
