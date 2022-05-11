using System;
using UnityEngine;
using static SoundService;

public class LifeableBrickController : IndestructibleBrickController
{
	[SerializeField] private int StartingLife;
	[SerializeField] private Particles DestroyParticles;
	[SerializeField] private Clips ExplosionSound;

	private int _currentLife;
	private int CurrentLife
	{
		get => _currentLife;
		set
		{
			_currentLife = value;
			OnLifeChanged?.Invoke(_currentLife);
		}
	}

	public event Action<int> OnLifeChanged;

	private void Start()
	{
		CurrentLife = StartingLife;
		GameManager.Instance.GetService<EventsService>().Raise(EventsService.Events.OnBrickPopped, new OnBrickPoppedEventArg() { Brick = this });
	}

	public bool CanAddLife() => (CurrentLife > 0) && (CurrentLife < StartingLife);

	public void AddLife(int life) => CurrentLife = Mathf.Clamp(CurrentLife + life, 0, StartingLife);

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		if (!Activated) return;

		AddLife(-1);

		if (CurrentLife > 0)
		{
			base.OnCollisionEnter2D(collision);
			return;
		}

		GameManager.Instance.GetService<SoundService>().Play(ExplosionSound);
		ParticleSystem particles = GameManager.Instance.GetService<ParticlesService>().Get(DestroyParticles, transform.position, Quaternion.FromToRotation(transform.up, -collision.GetContact(0).normal));
		ParticleSystem.MainModule mainModule = particles.main;
		mainModule.startSpeedMultiplier = collision.GetContact(0).normalImpulse;
		particles.Play();
		GameManager.Instance.GetService<EventsService>().Raise(EventsService.Events.OnBrickKilled, new OnBrickKilledEventArg() { Brick = this });
		gameObject.SetActive(false);
	}
}

