using TMPro;
using UnityEngine;
using static EventsService;

public class MenuController : MonoBehaviour
{
	[SerializeField] private GameObject BuyButton;

	private void Awake() => BuyButton.SetActive(GameManager.Instance.IAPService.IsBuyButtonActivated);

	private void Start()
	{
		if (GameManager.Instance.SoundService.IsMusicPlaying()) return;
		GameManager.Instance.SoundService.Play(GameManager.Instance.SoundService.GetNextMusic());
	}

	public void OnPlayClick()
	{
		GameManager.Instance.SoundService.StopMusic();
		GameManager.Instance.LevelService.CurrentLevelIndex = 0;
		GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Gameplay });
	}

	public void OnSelectClick() => GameManager.Instance.AdsService.ShowAd(GotoSelection);

	public void OnOptionClick() => GameManager.Instance.AdsService.ShowAd(GotoOptions);

	public void OnShopClick() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Shop });

	private void GotoSelection() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Selection });

	private void GotoOptions() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Options });
}
