using UnityEngine;

//[RequireComponent(typeof(IndestructibleBrickController))]
public class PopOnCollision : MonoBehaviour
{
	[SerializeField] private GameObject ObjectToPop;
	[SerializeField] private float PopRate;
	[SerializeField] private Tweening.Method Tweening;
	[SerializeField] private SplineDrawer Spline;
	[SerializeField] private float PopTime;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (Random.value > PopRate) return;

		Vector2 collisionVector = collision.contacts[0].normal * collision.contacts[0].normalImpulse;
		GameObject newGameObject = Instantiate(ObjectToPop, transform.position, Quaternion.identity);

		Rigidbody2D rb = newGameObject.AddComponent<Rigidbody2D>();
		rb.useFullKinematicContacts = true;
		rb.gravityScale = 0.0f;

		SetupCurveParameter setupCurve = new SetupCurveParameter();
		setupCurve.HasParameter = true;
		setupCurve.Spline = Instantiate(Spline, newGameObject.transform);
		setupCurve.InitialtPosition = transform.position;
		setupCurve.TargetPosition = transform.position + new Vector3(collisionVector.x, collisionVector.y);
		setupCurve.InitialRotation = Quaternion.LookRotation(Vector3.right, setupCurve.TargetPosition - transform.position);
		setupCurve.TargetRotation = setupCurve.InitialRotation;
		setupCurve.Timer = PopTime;
		setupCurve.Tweening = Tweening;

		CurveFollower curveFollower = CurveFollower.AddCurveFollower(newGameObject, setupCurve);
		curveFollower.OnDoneCommand = new DestroyComponentCommand(curveFollower);
		curveFollower.StartCurve();
	}
}
