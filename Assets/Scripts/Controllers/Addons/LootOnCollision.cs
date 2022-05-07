using UnityEngine;
using static EventsService;

public class LootOnCollision : MonoBehaviour
{
	[SerializeField] private GameObject ObjectToPop;
	[SerializeField] private Particles Particles;

	private void OnEnable() => GameManager.Instance.EventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);

	private void OnDisable() => GameManager.Instance.EventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);

	private void OnLevelEndedCallback(EventModelArg eventArg)
	{
		GameManager.Instance.ParticlesService.Get(Particles, transform.position).Play();
		Destroy(gameObject);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.CompareTag("Player"))
		{
			Instantiate(ObjectToPop, transform.position, Quaternion.identity);
			GameManager.Instance.ParticlesService.Get(Particles, transform.position).Play();
			Destroy(gameObject);
			return;
		}

		if (collision.collider.CompareTag("Ground"))
		{
			Destroy(gameObject);
			return;
		}
	}
}
