using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChromaticAberrationController : MonoBehaviour
{
	[SerializeField, Range(0.0f, 1.0f)] private float Intensity = 0.5f;
	[SerializeField] private float Duration = 0.5f;

	private Volume PostProcessingVolume;
	private ChromaticAberration chromaticAberration;

	private void Awake()
	{
		PostProcessingVolume = GetComponent<Volume>();
		PostProcessingVolume.profile.TryGet<ChromaticAberration>(out chromaticAberration);
	}

	private void OnEnable() => GameManager.Instance.GetService<EventsService>().Register(EventsService.Events.OnBallCollided, OnBallCollidedCallback);
	
	private void OnDisable() => GameManager.Instance.GetService<EventsService>().UnRegister(EventsService.Events.OnBallCollided, OnBallCollidedCallback);

	private void OnBallCollidedCallback(EventModelArg eventArg) => StartCoroutine(IntensityRoutine());

	private IEnumerator IntensityRoutine()
	{
		chromaticAberration.intensity.value = Intensity;
		float ttl = 0.0f;
		while (ttl < Duration)
		{
			chromaticAberration.intensity.value = Mathf.Lerp(1.0f, 0.0f, Tweening.QuintOut(ttl / Duration));
			yield return (null);
			ttl += Time.deltaTime;
		}
		chromaticAberration.intensity.value = 0.0f;
	}
}
