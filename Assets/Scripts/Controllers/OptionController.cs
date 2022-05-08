using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static EventsService;

public class OptionController : MonoBehaviour
{
	[SerializeField] private GameObject BuyButton;

	[SerializeField] private Toggle PostProcessingToggle;
	[SerializeField] private Slider QualitySlider;
	[SerializeField] private Slider GlobalVolume;
	[SerializeField] private Slider MusicVolume;
	[SerializeField] private Slider EffectVolume;
	[SerializeField] private TextMeshProUGUI QualityText;
	[SerializeField] private float EffectDelay = 0.5f;

	private WaitForSeconds WaitForSeconds;
	private bool CanPlayEffect = false;
	private LocalizeStringEvent QualityTextLocalized;

	private void Awake()
	{
		BuyButton.SetActive(GameManager.Instance.IAPService.IsBuyButtonActivated);
		WaitForSeconds = new WaitForSeconds(EffectDelay);
		QualityTextLocalized = QualityText.GetComponent<LocalizeStringEvent>();
	}

	private void Start()
	{
		PostProcessingToggle.isOn = GameManager.Instance.OptionsService.PostProcessing;
		QualitySlider.value = GameManager.Instance.OptionsService.GraphicQuality;
		GlobalVolume.value = GameManager.Instance.OptionsService.GlobalVolume;
		MusicVolume.value = GameManager.Instance.OptionsService.MusicVolume;
		EffectVolume.value = GameManager.Instance.OptionsService.EffectVolume;

		OnQualitySliderChanged(QualitySlider.value);

		CanPlayEffect = true;
	}

	public void OnBackClick() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });

	public void OnPostProcessingToggleChanged(bool newValue) => GameManager.Instance.OptionsService.PostProcessing = newValue;

	public void OnQualitySliderChanged(float newValue)
	{
		GameManager.Instance.OptionsService.GraphicQuality = (int)newValue;
		switch (GameManager.Instance.OptionsService.GraphicQuality)
		{
			case 0:
				QualityTextLocalized.SetEntry("LowQuality");
				break;

			case 1:
				QualityTextLocalized.SetEntry("MiddleQuality");
				break;

			case 2:
				QualityTextLocalized.SetEntry("HighQuality");
				break;

			case 3:
				QualityTextLocalized.SetEntry("MaxQuality");
				break;
		}
		QualityTextLocalized.RefreshString();
	}

	public void OnGlobalVolumeChanged(float newValue) => GameManager.Instance.OptionsService.GlobalVolume = (int)newValue;

	public void OnMusicVolumeChanged(float newValue) => GameManager.Instance.OptionsService.MusicVolume = (int)newValue;

	public void OnEffectVolumeChanged(float newValue)
	{
		GameManager.Instance.OptionsService.EffectVolume = (int)newValue;
		if (CanPlayEffect) StartCoroutine(PlayEffectRoutine());
	}

	private IEnumerator PlayEffectRoutine()
	{
		CanPlayEffect = false;
		GameManager.Instance.SoundService.Play(SoundService.Clips.Bonus);
		yield return (WaitForSeconds);
		CanPlayEffect = true;
	}

	public void OnShopClick() => GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Shop });
}
