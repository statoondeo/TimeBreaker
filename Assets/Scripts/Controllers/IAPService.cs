using UnityEngine.Purchasing;
using UnityEngine;
using static EventsService;

public class IAPService : MonoBehaviour, IStoreListener
{
	private static readonly string ProductID = "fullversionproduct";

	private IStoreController Controller;
	//private IExtensionProvider Extensions;

	public bool Initialized { get; private set; }
	public string PriceLabel { get; private set; }

	public bool IsBuyButtonActivated => !GameManager.Instance.IsCompleteMode && Initialized;

	private void Awake()
	{
		Initialized = false;
		ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		builder.AddProduct(ProductID, ProductType.NonConsumable);
		UnityPurchasing.Initialize(this, builder);
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		throw new System.NotImplementedException();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
	{
		GameManager.Instance.SwitchToCompleteVersion();
		GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });
		return (PurchaseProcessingResult.Complete);
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Controller = controller;
		Product product = Controller.products.WithID(ProductID);
		if ((null != product) && product.hasReceipt) GameManager.Instance.SwitchToCompleteVersion();
		PriceLabel = product.metadata.localizedPriceString;
		Initialized = true;
		GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });
	}

	public void InitiatePurchase() => Controller.InitiatePurchase(ProductID);

}