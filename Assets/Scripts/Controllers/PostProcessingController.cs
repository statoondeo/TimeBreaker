using UnityEngine;

public class PostProcessingController : MonoBehaviour
{
	[SerializeField] private GameObject PostProcessingObject;
	[SerializeField] private bool ConnectToOptions;

	private void Awake() => Init();

	private void OnEnable()
	{
		if (ConnectToOptions) GameManager.Instance.OptionsService.OnOptionsChanged += Init;
	}

	private void OnDisable()
	{
		if (ConnectToOptions) GameManager.Instance.OptionsService.OnOptionsChanged -= Init;
	}

	private void Init() => PostProcessingObject.SetActive(GameManager.Instance.OptionsService.PostProcessing);
}
