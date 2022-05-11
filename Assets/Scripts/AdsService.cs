using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsService : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener,  IService
{
	private static readonly float ADS_REFRESH = 180;
	private static readonly string GAME_ID = "4744033";
	private Action Callback;
	public float NextAdTime;

	private void Awake()
	{
		Initialize();
		NextAdTime = 0.0f;
	}

	private void Start() => Load();

	public void ShowAd(Action callback)
	{
		Callback = callback;
		if (GameManager.Instance.IsCompleteMode || (Time.realtimeSinceStartup < NextAdTime))
		{
			callback.Invoke();
			return;
		}
		NextAdTime = Time.realtimeSinceStartup + ADS_REFRESH;
		Advertisement.Show("Interstitial_Android", this);
	}

	private void Initialize() => Advertisement.Initialize(GAME_ID, false, this);

	private void Load() => Advertisement.Initialize(GAME_ID, true, this);

	public void OnInitializationComplete() { }

	public void OnInitializationFailed(UnityAdsInitializationError error, string message) { }

	public void OnUnityAdsAdLoaded(string placementId) { }

	public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) { }

	public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) { }

	public void OnUnityAdsShowStart(string placementId) { }

	public void OnUnityAdsShowClick(string placementId) { }

	public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) => Callback.Invoke();
}

