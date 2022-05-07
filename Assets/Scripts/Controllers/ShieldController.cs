using System.Collections;
using UnityEngine;
using static SoundService;

public class ShieldController : MonoBehaviour
{
	[SerializeField] private Vector3 InitialPosition = new Vector3(0.0f, -8.0f, 0.0f);
	[SerializeField] private Clips StartLoop;
	[SerializeField] private Clips EndLoop;

	private void OnEnable() => GameManager.Instance.EventsService.Register(EventsService.Events.OnPlayerShieldEnded, OnPlayerShieldEndedCallback);

	private void OnDisable() => GameManager.Instance.EventsService.UnRegister(EventsService.Events.OnPlayerShieldEnded, OnPlayerShieldEndedCallback);

	private void Start()
	{
		transform.position = InitialPosition;
		transform.localScale = Vector3.zero;
		GameManager.Instance.SoundService.Play(StartLoop);
		transform.ZoomTo(new Vector3(2.0f, 2.0f, 1.0f), 0.5f, Tweening.ElasticOut);
	}

	private void OnPlayerShieldEndedCallback(EventModelArg eventArg) => StartCoroutine(OnPlayerShieldEndedRoutine());

	private IEnumerator OnPlayerShieldEndedRoutine()
	{
		GameManager.Instance.SoundService.Play(EndLoop);
		yield return (transform.ZoomTo(Vector3.zero, 0.5f, Tweening.ElasticIn));
		Destroy(gameObject);
	}
}

