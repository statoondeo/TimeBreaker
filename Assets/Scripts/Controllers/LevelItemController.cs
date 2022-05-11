using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EventsService;

public class LevelItemController : MonoBehaviour
{
    [HideInInspector] public LevelModel LevelModel;

    [SerializeField] private RectTransform LockedPanel;
    [SerializeField] private RectTransform UnLockedPanel;
    [SerializeField] private RectTransform NoDataPanel;
    [SerializeField] private Image BronzeStar;
    [SerializeField] private Image SilverStar;
    [SerializeField] private Image GoldStar;
    [SerializeField] private TextMeshProUGUI BestText;
    [SerializeField] private TextMeshProUGUI NumberText;
    
	private CanvasGroup CanvasGroup;
	private bool Locked;

	private void Awake() => CanvasGroup = GetComponent<CanvasGroup>();

	public void Init(int index, LevelModel levelModel)
	{
        LevelModel =  levelModel;

        LevelsTimesSaver LevelsTimesSaver = GameManager.Instance.GetService<LevelsTimesSaver>();
        NumberText.text = LevelModel.Number.ToString("D2");

		Locked = LevelsTimesSaver.GetLocked(LevelModel.Id);
		if (Locked)
		{
			NumberText.color = Color.red;
			LockedPanel.gameObject.SetActive(true);
			StartCoroutine(AlphaToRoutine(index));
			return;
		}

		if (LevelsTimesSaver.HasTimes(LevelModel.Id))
		{
			UnLockedPanel.gameObject.SetActive(true);
			float best = LevelsTimesSaver.GetBest(LevelModel.Id, levelModel.SuccessTimer);

			if (best <= levelModel.BronzeTimer)
			{
				BronzeStar.gameObject.SetActive(true);
				if (best <= levelModel.SilverTimer)
				{
					SilverStar.gameObject.SetActive(true);
					if (best <= levelModel.GoldTimer) GoldStar.gameObject.SetActive(true);
				}
			}

			BestText.text = TimerController.GetFormattedTime(LevelsTimesSaver.GetBest(LevelModel.Id, LevelModel.SuccessTimer));
			StartCoroutine(AlphaToRoutine(index));
			return;
		}

		NoDataPanel.gameObject.SetActive(true);
		StartCoroutine(AlphaToRoutine(index));
	}

	private IEnumerator AlphaToRoutine(int index)
	{
		yield return (new WaitForSeconds(index * 0.1f));
		CanvasGroup.AlphaTo(1.0f, 0.5f, Tweening.QuintOut);
	}

	public void OnPlayClick()
    {
		if (Locked) return;
        GameManager.Instance.GetService<SoundService>().StopMusic();
		GameManager.Instance.GetService<LevelService>().CurrentLevelIndex = LevelModel.Number - 1;
        GameManager.Instance.GetService<EventsService>().Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Gameplay });
    }
}
