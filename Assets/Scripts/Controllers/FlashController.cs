using System.Collections;
using UnityEngine;

public class FlashController : MonoBehaviour
{
	[SerializeField] private float Duration;

	private SpriteRenderer SpriteRenderer;

	private void Awake()
	{
		SpriteRenderer = GetComponent<SpriteRenderer>();
		SpriteRenderer.enabled = false;
	}

	public void StartFlash(Color color) => StartCoroutine(FlashRoutine(color));

	private IEnumerator FlashRoutine(Color targetColor)
	{
		SpriteRenderer.enabled = true;
		SpriteRenderer.color = targetColor;
		Color finalColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0.0f);
		yield return (SpriteRenderer.ColorTo(finalColor, Duration, Tweening.QuintInOut));
		SpriteRenderer.enabled = false;
	}
}
