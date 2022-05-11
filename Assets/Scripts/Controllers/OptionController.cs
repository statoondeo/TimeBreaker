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
	[SerializeField] private Slider SensibilitySlider;
	[SerializeField] private TextMeshProUGUI QualityText;
	[SerializeField] private TextMeshProUGUI SensibilityText;
	[SerializeField] private float EffectDelay = 0.5f;

	private WaitForSeconds WaitForSeconds;
	private bool CanPlayEffect = false;
	private LocalizeStringEvent QualityTextLocalized;
	private LocalizeStringEvent SensibilityTextLocalized;

	private void Awake()
	{
		BuyButton.SetActive(GameManager.Instance.GetService<IAPService>().IsBuyButtonActivated);
		WaitForSeconds = new WaitForSeconds(EffectDelay);
		QualityTextLocalized = QualityText.GetComponent<LocalizeStringEvent>();
		SensibilityTextLocalized = SensibilityText.GetComponent<LocalizeStringEvent>();
	}

	private void Start()
	{
		OptionsService optionsService = GameManager.Instance.GetService<OptionsService>();
		PostProcessingToggle.isOn = optionsService.PostProcessing;
		QualitySlider.value = optionsService.GraphicQuality;
		GlobalVolume.value = optionsService.GlobalVolume;
		MusicVolume.value = optionsService.MusicVolume;
		EffectVolume.value = optionsService.EffectVolume;
		SensibilitySlider.value = optionsService.Sensibility;

		OnQualitySliderChanged(QualitySlider.value);

		CanPlayEffect = true;
	}

	public void OnBackClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });

	public void OnPostProcessingToggleChanged(bool newValue) => GameManager.Instance.GetService<OptionsService>().PostProcessing = newValue;

	public void OnSensibilitySliderChanged(float newValue)
	{
		OptionsService optionsService = GameManager.Instance.GetService<OptionsService>();
		optionsService.Sensibility = (int)newValue;
		switch (optionsService.Sensibility)
		{
			case 0:
				SensibilityTextLocalized.SetEntry("LowSensitivity");
				break;

			case 1:
				SensibilityTextLocalized.SetEntry("MiddleSensitivity");
				break;

			case 2:
				SensibilityTextLocalized.SetEntry("HighSensitivity");
				break;
		}
		SensibilityTextLocalized.RefreshString();
	}

	public void OnQualitySliderChanged(float newValue)
	{
		OptionsService optionsService = GameManager.Instance.GetService<OptionsService>();
		optionsService.GraphicQuality = (int)newValue;
		switch (optionsService.GraphicQuality)
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

	public void OnGlobalVolumeChanged(float newValue) => GameManager.Instance.GetService<OptionsService>().GlobalVolume = (int)newValue;

	public void OnMusicVolumeChanged(float newValue) => GameManager.Instance.GetService<OptionsService>().MusicVolume = (int)newValue;

	public void OnEffectVolumeChanged(float newValue)
	{
		GameManager.Instance.GetService<OptionsService>().EffectVolume = (int)newValue;
		if (CanPlayEffect) StartCoroutine(PlayEffectRoutine());
	}

	private IEnumerator PlayEffectRoutine()
	{
		CanPlayEffect = false;
		GameManager.Instance.GetService<SoundService>().Play(SoundService.Clips.Bonus);
		yield return (WaitForSeconds);
		CanPlayEffect = true;
	}

	public void OnShopClick() => GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Shop });
}
