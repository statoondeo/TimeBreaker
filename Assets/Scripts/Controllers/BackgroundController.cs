using UnityEngine;

public class BackgroundController : MonoBehaviour
{
	[SerializeField] private SpriteRenderer Background;

	private void Awake() => Background.sprite = GameManager.Instance.GetService<BackgroundsService>().GetNext();
}
