using UnityEngine;
using static SoundService;

public class TimeBonusController : MonoBehaviour
{
	[SerializeField] private float Duration = 3.0f;
	[SerializeField] private Clips Sound;

	private void Start()
	{
		GameManager.Instance.SoundService.Play(Sound);
		GameManager.Instance.EventsService.Raise(EventsService.Events.OnTimerPaused, new OnTimerPausedEventArg() { Duration = Duration });
		Destroy(gameObject);
	}
}
