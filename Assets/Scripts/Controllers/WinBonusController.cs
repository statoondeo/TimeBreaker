using System.Collections;
using UnityEngine;

public class WinBonusController : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return (new WaitForSeconds(2.0f));
		GameManager.Instance.GetService<EventsService>().Raise(EventsService.Events.OnBricksEnded);
		Destroy(gameObject);
	}
}
