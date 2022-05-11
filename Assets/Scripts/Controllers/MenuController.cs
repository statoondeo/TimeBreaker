using TMPro;
using UnityEngine;
using static EventsService;

public class MenuController : MonoBehaviour
{
	[SerializeField] private GameObject BuyButton;

	private void Awake() => BuyButton.SetActive(GameManager.Instance.GetService<IAPService>().IsBuyButtonActivated);

	private void Start()
	{
		SoundService soundService = GameManager.Instance.GetService<SoundService>();
		if (soundService.IsMusicPlaying()) return;
		soundService.Play(GameManager.Instance.GetService<SoundService>().GetNextMusic());
	}

	public void OnPlayClick()
	{
		GameManager.Instance.GetService<SoundService>().StopMusic();
		GameManager.Instance.GetService<LevelService>().CurrentLevelIndex = 0;
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Gameplay });
	}

	public void OnSelectClick() => GameManager.Instance.GetService<AdsService>().ShowAd(GotoSelection);

	public void OnOptionClick() => GameManager.Instance.GetService<AdsService>().ShowAd(GotoOptions);

	public void OnShopClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Shop });

	private void GotoSelection() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Selection });

	private void GotoOptions() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Options });
}
