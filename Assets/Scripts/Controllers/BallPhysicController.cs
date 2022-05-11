using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallPhysicController : MonoBehaviour
{
	private bool _Activated;
	public bool Activated
	{
		get => _Activated;
		set
		{
			if (value != _Activated)
			{
				_Activated = value;

				Collider.enabled = _Activated;
				Rigidbody.isKinematic = !_Activated;
			}
		}
	}

	[SerializeField] private float MaxSpeed;
	[SerializeField] private float VerticalTolerance = 0.1f;

	private Rigidbody2D Rigidbody;
	private Collider2D Collider;

	private void Awake()
	{
		Collider = GetComponent<Collider2D>();
		Rigidbody = GetComponent<Rigidbody2D>();

		Activated = true;
	}

	private void FixedUpdate()
	{
		if (!Activated) return;

		Rigidbody.velocity = Vector2.ClampMagnitude(Rigidbody.velocity, MaxSpeed);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!Activated) return;

		if (Rigidbody.velocity.x <= VerticalTolerance) Rigidbody.velocity = new Vector2(Rigidbody.velocity.x + Mathf.Sign(Rigidbody.velocity.x) * VerticalTolerance, Rigidbody.velocity.y);

		Rigidbody.velocity += 0.5f * MaxSpeed * Rigidbody.velocity;
	}
}

