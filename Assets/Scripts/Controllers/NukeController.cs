using System.Collections;
using UnityEngine;
using static SoundService;

public class NukeController : MonoBehaviour
{
	[SerializeField] private float Duration = 2.0f;
	[SerializeField] private float Size = 5.0f;

	[SerializeField] private Clips Sound;

	private void Start()
	{
		StartCoroutine(NukeWaveRoutine());
	}

	private IEnumerator NukeWaveRoutine()
	{
		transform.localScale = Vector3.zero;
		GameManager.Instance.GetService<SoundService>().Play(Sound);
		yield return (transform.ZoomTo(new Vector3(Size, Size, 1.0f), Duration, Tweening.QuintIn));
		yield return (new WaitForSeconds(Duration));
		Destroy(gameObject);
	}
}
