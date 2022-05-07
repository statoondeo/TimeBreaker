using System;
using UnityEngine;

//[RequireComponent(typeof(Collider2D))]
public class PaddlePhysicController : MonoBehaviour
{
	[SerializeField, Range(1.0f, 8.0f)] private int Amplitude = 6;
	[SerializeField, Range(0.5f, 5.0f)] private float Bounciness;

	public event Action<Rigidbody2D> OnHit;
	private Collider2D Collider;
	private bool _Activated;
	private float MinVerticalVelocity;

	public bool Activated
	{
		get => _Activated;
		set
		{
			_Activated = value;
			Collider.enabled = _Activated;
		}
	}

	private void Awake()
	{
		Collider = GetComponent<Collider2D>();
		Activated = false;
	}

	private void Start() => MinVerticalVelocity = Mathf.Sin(Mathf.PI / Amplitude);


	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collision.collider.gameObject.CompareTag("Ball")) return;

		float initialMagnitude = collision.rigidbody.velocity.magnitude;

		Vector3 paddleDirection = new Vector2(collision.collider.transform.position.x - collision.otherCollider.transform.position.x, collision.collider.transform.position.y - collision.otherCollider.transform.position.y);
		Vector2 newNormalizedVelocity = paddleDirection.normalized;
		if (newNormalizedVelocity.y < MinVerticalVelocity)
		{
			newNormalizedVelocity.x = Mathf.Sign(newNormalizedVelocity.x) * Mathf.Cos(Mathf.PI / Amplitude);
			newNormalizedVelocity.y = MinVerticalVelocity;
		}
		collision.rigidbody.velocity = Bounciness * initialMagnitude * newNormalizedVelocity;
		OnHit?.Invoke(collision.rigidbody);
	}
}
