using UnityEngine.Purchasing;
using UnityEngine;
using static EventsService;

public class IAPService : MonoBehaviour, IStoreListener, IService
{
	private static readonly string ProductID = "fullversionproduct";

	private IStoreController Controller;

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

	public void OnInitializeFailed(InitializationFailureReason error) => Initialized = false;

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
	{
		GameManager.Instance.IsCompleteMode = true;
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });
		return (PurchaseProcessingResult.Complete);
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
#if UNITY_WEBGL
		Initialized = false;
#else
		Controller = controller;
		Product product = Controller.products.WithID(ProductID);
		GameManager.Instance.IsCompleteMode = (null != product) && product.hasReceipt;
		PriceLabel = product.metadata.localizedPriceString;
		Initialized = true;
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });
#endif
	}

	public void InitiatePurchase() => Controller.InitiatePurchase(ProductID);

}