using UnityEngine;

public class ShieldBonusController : MonoBehaviour
{
	[SerializeField] private float Duration = 5.0f;

	private void Start()
	{
		GameManager.Instance.GetService<EventsService>().Raise(EventsService.Events.OnPlayerShieldStarted, new OnPlayerShieldStartedEventArg() { Duration = Duration });
		Destroy(gameObject);
	}
}
