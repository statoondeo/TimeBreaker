using UnityEngine;

public class PanelAds : MonoBehaviour
{
	[SerializeField] private RectTransform AdsPanel;

	private void Awake()
	{
#if UNITY_WEBGL
		AdsPanel.gameObject.SetActive(false);
#else
		AdsPanel.gameObject.SetActive(!GameManager.Instance.IsCompleteMode);
#endif
	}
}
