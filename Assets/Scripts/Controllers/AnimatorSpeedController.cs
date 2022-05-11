using UnityEngine;

public class AnimatorSpeedController : MonoBehaviour
{
	[SerializeField] private float AnimationSpeed = 1.0f;

	private Animator Animator;

	private void Awake() => Animator = GetComponent<Animator>();

	private void Start() => Animator.SetFloat("Speed", AnimationSpeed);
}
