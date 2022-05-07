using UnityEngine;

public class OutOfBoundController : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.TryGetComponent<BallController>(out BallController ball)) ball.Kill();
	}
}
