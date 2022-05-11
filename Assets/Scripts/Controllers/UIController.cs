using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventsService;
using static SoundService;

public class UIController : MonoBehaviour
{
	[SerializeField] private FlashController FlashController;

	[SerializeField] private CanvasGroup TimerPanel;
	[SerializeField] private CanvasGroup InGamePanel;
	[SerializeField] private CanvasGroup VictoryPanel;
	[SerializeField] private CanvasGroup DefeatPanel;

	[SerializeField] private CanvasGroup TitlePanel;
	[SerializeField] private CanvasGroup StartPanel;

	[SerializeField] private TextMeshProUGUI TimeText;
	[SerializeField] private TextMeshProUGUI BestText;
	[SerializeField] private TextMeshProUGUI LevelText;
	[SerializeField] private GameObject Swipes;
	[SerializeField] private TextMeshProUGUI VictoryIconText;
	[SerializeField] private TextMeshProUGUI DefeatIconText;

	[SerializeField] private Image BronzeStar;
	[SerializeField] private Image SilverStar;
	[SerializeField] private Image GoldStar;

	[SerializeField] private Particles StarParticles;

	[SerializeField] private Clips CountdownAlertSound;
	[SerializeField] private Clips CountdownFinalSound;
	[SerializeField] private Clips StarSound;

	private TextMeshProUGUI StartText;
	private CanvasGroup FinalPanel;
	private LevelModel LevelModel;

	[SerializeField] private GameObject BuyButton;

	private void Awake()
	{
		StartText = StartPanel.GetComponentInChildren<TextMeshProUGUI>();
		StartPanel.gameObject.SetActive(false);
		TitlePanel.gameObject.SetActive(false);
		VictoryPanel.gameObject.SetActive(false);
		DefeatPanel.gameObject.SetActive(false);

		BuyButton.SetActive(false);
	}

	private void OnEnable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.Register(Events.OnLevelSetupStarted, OnLevelSetupStartedCallback);
		eventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnDisable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnLevelSetupStarted, OnLevelSetupStartedCallback);
		eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnLevelEndedCallback(EventModelArg eventArg)
	{
		OnLevelEndedEventArg onLevelEndedEventArg = eventArg as OnLevelEndedEventArg;
		if (null != eventArg)
		{
			TimeText.text = TimerController.GetFormattedTime(onLevelEndedEventArg.Score);
			BestText.text = TimerController.GetFormattedTime(onLevelEndedEventArg.Best);
			if (onLevelEndedEventArg.Score == onLevelEndedEventArg.Best)
			{
				TimeText.color = new Color(1.0f, 0.65f, 0.0f, 1.0f);
				BestText.color = TimeText.color;
			}
			FinalPanel = VictoryPanel;

			if (onLevelEndedEventArg.Score <= LevelModel.BronzeTimer)
			{
				StartCoroutine(DisplayStar(0.5f, BronzeStar.gameObject));
				if (onLevelEndedEventArg.Score <= LevelModel.SilverTimer)
				{
					StartCoroutine(DisplayStar(1.0f, SilverStar.gameObject));
					if (onLevelEndedEventArg.Score <= LevelModel.GoldTimer)
					{
						StartCoroutine(DisplayStar(1.5f, GoldStar.gameObject));
					}
				}
			}
		}
		else
		{
			FinalPanel = DefeatPanel;
		}
		StartCoroutine(DisplayFinalPanel(LevelModel.Number));
	}

	private void OnLevelSetupStartedCallback(EventModelArg eventArg)
	{
		LevelModel = (eventArg as OnSetupStartedEventArg).LevelModel;
		StartCoroutine(DisplayTitleText(LevelModel.Name));
		StartCoroutine(DisplayStartText());
		StartCoroutine(DisplayInGameUI());
	}

	private IEnumerator DisplayStar(float delay, GameObject star)
	{
		yield return (new WaitForSeconds(2.0f + delay));
		GameManager.Instance.GetService<SoundService>().Play(StarSound);
		star.SetActive(true);
		Vector3 initialScale = star.transform.localScale;
		star.transform.localScale = Vector3.zero;
		star.transform.ZoomTo(initialScale, 0.5f, Tweening.ElasticOut);
		GameManager.Instance.GetService<ParticlesService>().Get(StarParticles, star.transform.position).Play();
	}

	private IEnumerator DisplayFinalPanel(int levelNumber)
	{
		FlashController.StartFlash(FinalPanel == DefeatPanel ? new Color(Color.red.r, Color.red.g, Color.red.b, 0.75f) : new Color(Color.white.r, Color.white.g, Color.white.b, 0.75f));
		yield return (new WaitForSeconds(2.0f));
		FinalPanel.gameObject.SetActive(true);
		FinalPanel.alpha = 0.0f;
		VictoryIconText.text = DefeatIconText.text = levelNumber.ToString("D2");
		GameManager.Instance.GetService<SoundService>().Play(FinalPanel == DefeatPanel ? Clips.Fail : Clips.Victory);
		FinalPanel.AlphaTo(1.0f, 1.0f, Tweening.QuintOut);
		BuyButton.SetActive(GameManager.Instance.GetService<IAPService>().IsBuyButtonActivated);
		Swipes.SetActive(false);
	}

	private IEnumerator DisplayInGameUI()
	{
		TimerPanel.alpha = 0.0f;
		InGamePanel.alpha = 0.0f;

		yield return (new WaitForSeconds(2.0f));

		TimerPanel.AlphaTo(1.0f, 3.0f, Tweening.QuintOut);
		InGamePanel.AlphaTo(1.0f, 3.0f, Tweening.QuintOut);
	}

	private IEnumerator DisplayTitleText(string levelName)
	{
		yield return (new WaitForSeconds(1.0f));
		TitlePanel.gameObject.SetActive(true);
		TitlePanel.alpha = 0.0f;
		LevelText.text = levelName;
		yield return (TitlePanel.AlphaTo(1.0f, .25f, Tweening.QuintOut));
		yield return (new WaitForSeconds(3.5f));
		yield return (TitlePanel.AlphaTo(0.0f, .25f, Tweening.QuintIn));

		TitlePanel.gameObject.SetActive(false);
	}

	private IEnumerator DisplayStartText()
	{
		yield return (new WaitForSeconds(1.0f));
		StartPanel.gameObject.SetActive(true);
		StartPanel.alpha = 0.0f;
		string startingText;
		Clips sound;
		for (int i = 3; i >= 0; i--)
		{
			if (i == 0)
			{
				startingText = "Start";
				sound = CountdownFinalSound;
			}
			else
			{
				startingText = i.ToString();
				sound = CountdownAlertSound;
			}
			StartText.text = startingText;
			GameManager.Instance.GetService<SoundService>().Play(sound);
			yield return (StartPanel.AlphaTo(1.0f, .25f, Tweening.QuintOut));
			yield return (new WaitForSeconds(.4f));
			yield return (StartPanel.AlphaTo(0.0f, .25f, Tweening.QuintIn));
			yield return (new WaitForSeconds(.1f));
		}

		StartPanel.gameObject.SetActive(false);
	}
}
