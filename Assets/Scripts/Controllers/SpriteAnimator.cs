using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimator : MonoBehaviour
{
	[SerializeField] private Sprite[] Sprites;
	[SerializeField] private float FrameRate;

	private int CurrentFrame = 0;
	private SpriteRenderer SpriteRenderer;
	private int NbSprites;

	[HideInInspector] public Color CurrentColor = Color.white;

	private void Awake()
{
		SpriteRenderer = GetComponent<SpriteRenderer>();
		NbSprites = Sprites.Length;
	}

	private void Start()
	{
		if ((NbSprites > 1) && (FrameRate > 0)) InvokeRepeating(nameof(ChangeSprite), 0.0f, FrameRate);
	}

	private void OnDisable() => CancelInvoke(nameof(ChangeSprite));


	private void Update() => SpriteRenderer.color = CurrentColor;

	private void ChangeSprite()
	{
		SpriteRenderer.sprite = Sprites[CurrentFrame];
		CurrentFrame = (CurrentFrame + 1) % NbSprites;
	}
}
