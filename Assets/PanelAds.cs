using UnityEngine;

public class PanelAds : MonoBehaviour
{
	[SerializeField] private RectTransform AdsPanel;

	private void Awake() => AdsPanel.gameObject.SetActive(!GameManager.Instance.IsCompleteMode);
}
