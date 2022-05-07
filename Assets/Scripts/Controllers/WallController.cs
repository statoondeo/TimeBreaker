using UnityEngine;
using static SoundService;

public class WallController : MonoBehaviour
{
	[SerializeField] private Particles CollisionParticles;
	[SerializeField] private Clips CollisionSound;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		GameManager.Instance.ParticlesService.Get(CollisionParticles, collision.GetContact(0).point).Play();
		GameManager.Instance.SoundService.Play(CollisionSound);
	}
}

