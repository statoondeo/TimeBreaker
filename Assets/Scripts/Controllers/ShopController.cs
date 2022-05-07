using TMPro;
using UnityEngine;
using static EventsService;

public class ShopController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI Pricelabel;

	private void Awake() => Pricelabel.text = GameManager.Instance.IAPService.PriceLabel;

	public void OnBackClick() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });

	public void OnBuyClick() => GameManager.Instance.IAPService.InitiatePurchase();
}
