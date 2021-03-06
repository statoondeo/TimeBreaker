using TMPro;
using UnityEngine;
using static EventsService;

public class ShopController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI Pricelabel;
	[SerializeField] private GameObject AdLabel;
	[SerializeField] private GameObject AdButton;

	private void Awake()
	{
		Pricelabel.text = GameManager.Instance.GetService<IAPService>().PriceLabel;
	}

	public void OnBackClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });

	public void OnBuyClick() => GameManager.Instance.GetService<IAPService>().InitiatePurchase();

	public void OnAdClick() => GameManager.Instance.GetService<AdsService>().ShowAd();
}
