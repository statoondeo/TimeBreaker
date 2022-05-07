using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LifeBadgeController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI LifeText;
	[SerializeField] private LifeableBrickController LifeableBrickController;
	[SerializeField] private Image Image;
	[SerializeField] private float ZoomFactor = 0.5f;
	[SerializeField] private float Duration = 0.5f;

	private Vector3 InitialScale;
	private Vector3 TargetScale;

	private void Awake()
	{
		InitialScale = Image.transform.localScale;
		TargetScale = new Vector3(InitialScale.x * ZoomFactor, InitialScale.y * ZoomFactor, 1.0f);
		LifeableBrickController.OnLifeChanged += OnLifeChangedCallback;
	}

	private void OnLifeChangedCallback(int newLife)
	{
		LifeText.text = newLife.ToString();
		Image.transform.localScale = TargetScale;
		Image.transform.ZoomTo(InitialScale, Duration, Tweening.ElasticOut);
	}
}
