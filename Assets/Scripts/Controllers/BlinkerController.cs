using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(SpriteAnimator))]
public class BlinkerController : MonoBehaviour
{
	public float BlinkTransitionDuration = 0.125f;
	public float BlinkDuration = 0.25f;
	public Color BlinkColor = Color.red;

	[HideInInspector] public Color OriginalColor;
	private SpriteAnimator SpriteAnimator;
	private WaitForSeconds WaitForSeconds;

	private void Awake()
	{
		SpriteAnimator = GetComponent<SpriteAnimator>();
		OriginalColor = SpriteAnimator.CurrentColor;
		WaitForSeconds = new WaitForSeconds(BlinkDuration);
	}

	public IEnumerator BlinkRoutine()
	{
		yield return (BlinkColorRoutine(OriginalColor, BlinkColor, BlinkTransitionDuration));
		yield return (WaitForSeconds);
		yield return (BlinkColorRoutine(BlinkColor, OriginalColor, BlinkTransitionDuration));
	}

	public IEnumerator BlinkColorRoutine(Color originalColor, Color targetColor, float duration)
	{
		float ttl = 0.0f;
		while (ttl < duration)
		{
			SpriteAnimator.CurrentColor = Color.Lerp(originalColor, targetColor, ttl / duration);
			yield return (null);
			ttl += Time.deltaTime;
		}
		SpriteAnimator.CurrentColor = targetColor;
	}
}
