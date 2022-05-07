using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Tweening
{
	public static Func<float, float> GetMethod(Method method)
	{
		return method switch
		{
			Method.Linear => (Tweening.Lin),
			Method.SinOut => (Tweening.SinOut),
			Method.SinInOut => (Tweening.SinInOut),
			Method.QuintIn => (Tweening.QuintIn),
			Method.QuintOut => (Tweening.QuintOut),
			Method.QuintInOut => (Tweening.QuintInOut),
			Method.ElasticIn => (Tweening.ElasticIn),
			Method.ElasticOut => (Tweening.ElasticOut),
			Method.CubicIn => (Tweening.CubicIn),
			_ => (null),
		};
	}

	public enum Method
	{
		Linear = 0,

		SinOut = 11,
		SinInOut = 12,

		QuintIn = 20,
		QuintOut = 21,
		QuintInOut = 22,

		ElasticOut = 30,
		ElasticIn = 31,

		CubicIn = 40,
	}

	#region Méthodes exposées

	public static Coroutine MoveTo(this RectTransform rectTransform, Vector2 targetPosition, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(MoveToRoutine(rectTransform, targetPosition, duration, tweening)));
	}

	public static Coroutine MoveTo(this Transform transform, Vector3 targetPosition, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(MoveToRoutine(transform, targetPosition, duration, tweening)));
	}

	public static Coroutine ZoomTo(this Transform transform, Vector3 targetZoom, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(ZoomToRoutine(transform, targetZoom, duration, tweening)));
	}

	public static Coroutine ColorTo(this SpriteRenderer sprite, Color targetColor, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(ColorToRoutine(sprite, targetColor, duration, tweening)));
	}

	public static Coroutine ColorTo(this Image image, Color targetColor, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(ColorToRoutine(image, targetColor, duration, tweening)));
	}

	public static Coroutine ColorTo(this TextMeshProUGUI textMeshProUGUI, Color targetColor, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(ColorToRoutine(textMeshProUGUI, targetColor, duration, tweening)));
	}

	public static Coroutine ColorTo(this LineRenderer lineRenderer, Color targetColor, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(ColorToRoutine(lineRenderer, targetColor, duration, tweening)));
	}

	public static Coroutine AlphaTo(this CanvasGroup canvasGroup, float targetValue, float duration, Func<float, float> tweening)
	{
		return (GameManager.Instance.StartCoroutine(AlphaToRoutine(canvasGroup, targetValue, duration, tweening)));
	}

	#endregion

	#region Fonctions de tweening

	public static float Lin(float progress)
	{
		return (progress);
	}

	public static float SinOut(float progress)
	{
		return (Mathf.Sin((Mathf.PI * progress) / 2));
	}

	public static float SinInOut(float progress)
	{
		return (-(Mathf.Cos(Mathf.PI * progress) - 1) / 2);
	}

	public static float CubicIn(float progress)
	{
		return (progress * progress * progress);
	}
	public static float QuintIn(float progress)
	{
		return (Mathf.Pow(progress, 5));
	}

	public static float QuintOut(float progress)
	{
		return (1 - Mathf.Pow(1 - progress, 5));
	}

	public static float QuintInOut(float progress)
	{
		return (progress < 0.5f ? 16.0f * Mathf.Pow(progress, 5) : 1 - Mathf.Pow(-2 * progress + 2, 5) / 2);
	}

	public static float ElasticOut(float progress)
	{
		const float c4 = (2 * Mathf.PI) / 3;
		return (progress == 0 ? 0 : progress == 1 ? 1 : Mathf.Pow(2, -10 * progress) * Mathf.Sin((progress * 10 - 0.75f) * c4) + 1);
	}

	public static float ElasticIn(float progress)
	{
		const float c4 = (2 * Mathf.PI) / 3;
		return (progress == 0 ? 0 : progress == 1 ? 1 : -Mathf.Pow(2, 10 * progress - 10) * Mathf.Sin((progress * 10 - 10.75f) * c4));
	}

	#endregion

	#region Coroutines de transitions

	public static IEnumerator MoveToRoutine(this RectTransform rectTransform, Vector2 targetPosition, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		Vector2 originPosition = rectTransform.anchoredPosition;
		while (ttl < duration)
		{
			rectTransform.anchoredPosition = Vector2.Lerp(originPosition, targetPosition, tweening(ttl / duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		rectTransform.anchoredPosition = targetPosition;
	}

	public static IEnumerator MoveToRoutine(this Transform transform, Vector3 targetPosition, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		Vector3 originPosition = transform.position;
		while (ttl < duration)
		{
			if (null == transform) yield break;
			transform.position = Vector3.Lerp(originPosition, targetPosition, tweening(ttl / duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		transform.position = targetPosition;
	}

	public static IEnumerator ZoomToRoutine(Transform transform, Vector3 targetZoom, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		Vector3 originZoom = transform.localScale;
		while (ttl < duration)
		{
			transform.localScale = Vector3.Lerp(originZoom, targetZoom, tweening(ttl / duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		transform.localScale = targetZoom;
	}

	public static IEnumerator ColorToRoutine(SpriteRenderer sprite, Color targetColor, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		Color originColor = sprite.material.color;
		while (ttl < duration)
		{
			sprite.material.color = Color.Lerp(originColor, targetColor, tweening(ttl / duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		sprite.material.color = targetColor;
	}

	public static IEnumerator ColorToRoutine(Image image, Color targetColor, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		Color originColor = image.color;
		while (ttl < duration)
		{
			image.color = Color.Lerp(originColor, targetColor, tweening(ttl / duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		image.color = targetColor;
	}

	public static IEnumerator ColorToRoutine(TextMeshProUGUI textMeshProUGUI, Color targetColor, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		Color originColor = textMeshProUGUI.color;
		while (ttl < duration)
		{
			textMeshProUGUI.color = Color.Lerp(originColor, targetColor, tweening(ttl / duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		textMeshProUGUI.color = targetColor;
	}

	public static IEnumerator ColorToRoutine(this LineRenderer lineRenderer, Color targetColor, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		Color originColor = lineRenderer.startColor;
		while (ttl < duration)
		{
			lineRenderer.startColor = Color.Lerp(originColor, targetColor, tweening(ttl / duration));
			lineRenderer.endColor = lineRenderer.startColor;
			yield return (null);
			ttl += Time.deltaTime;
		}
		lineRenderer.startColor = targetColor;
		lineRenderer.endColor = lineRenderer.startColor;
	}

	public static IEnumerator AlphaToRoutine(CanvasGroup canvasGroup, float targetValue, float duration, Func<float, float> tweening)
	{
		float ttl = 0.0f;
		float originalValue = canvasGroup.alpha;
		while (ttl < duration)
		{
			canvasGroup.alpha = Mathf.Lerp(originalValue, targetValue, tweening(ttl / duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		canvasGroup.alpha = targetValue;
	}

	#endregion
}