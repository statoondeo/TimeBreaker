using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer))]
public class SpriteColorAnimator : MonoBehaviour
{
	[SerializeField] private float AnimatorSpeed;
	public Color CurrentColor = Color.white;

	private SpriteRenderer SpriteRenderer;
	private Animator Animator;

	private void Awake()
	{
		CurrentColor = Color.white;
		SpriteRenderer = GetComponent<SpriteRenderer>();
		Animator = GetComponent<Animator>();
		if (null != Animator) Animator.speed = AnimatorSpeed;
	}

	private void Update() => SpriteRenderer.color = CurrentColor;
}
