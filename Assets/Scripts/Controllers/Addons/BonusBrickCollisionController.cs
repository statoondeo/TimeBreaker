using UnityEngine;
using static SoundService;

//[RequireComponent(typeof(IndestructibleBrickController))]
public class BonusBrickCollisionController : IndestructibleBrickController
{
	[SerializeField] private BadgeController BadgeController;
	[SerializeField] private Particles LootParticles;
	[SerializeField] private Clips BonusSound;

	protected override void OnCollisionEnter2D(Collision2D collision)
{
		if (!Activated) return;

		if (!BadgeController.CanStart)
		{
			base.OnCollisionEnter2D(collision);
			return;
		}

		GameManager.Instance.GetService<SoundService>().Play(BonusSound);
		BadgeController.StartBadgeTimer();

		Rigidbody2D rigidbody2D = Instantiate(GameManager.Instance.GetService<LootService>().GetNextLoot(), transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
		rigidbody2D.velocity = 2.0f * collision.contacts[0].normalImpulse * collision.contacts[0].normal;

		ParticleSystem particles = GameManager.Instance.GetService<ParticlesService>().Get(LootParticles, transform.position, Quaternion.FromToRotation(transform.up, -collision.contacts[0].normal));
		ParticleSystem.MainModule mainModule = particles.main;
		mainModule.startSpeedMultiplier = collision.contacts[0].normalImpulse;
		particles.Play();
	}
}
