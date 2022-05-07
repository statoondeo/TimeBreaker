using System.Collections;
using UnityEngine;

//[RequireComponent(typeof(SpriteAnimator))]
public class LoopingBlinker : MonoBehaviour
{
    private BlinkerController BlinkerController;
	private int NbParams;
	private int CurrentParamIndex = 0;

	[SerializeField] private BlinkLoopParam[] BlinkLoopParams;
	[SerializeField] private bool Randomize;

	private void Awake() => NbParams = BlinkLoopParams.Length;

	private void Start() {
		if (Randomize)
			StartCoroutine(StartRandomBlinker());
		else
			StartCoroutine(StartBlinker());
	}

	private IEnumerator StartRandomBlinker()
	{
		BlinkerController = gameObject.AddComponent<BlinkerController>();
		BlinkerController.BlinkColor = new Color(Random.value, Random.value, Random.value);
		BlinkerController.BlinkTransitionDuration = 4.0f + Random.value * 8.0f;
		BlinkerController.BlinkDuration = 2.0f + Random.value * 4.0f;
		yield return (StartCoroutine(BlinkerController.BlinkRoutine()));
		Destroy(BlinkerController);
		yield return (StartCoroutine(StartRandomBlinker()));
	}

	private IEnumerator StartBlinker()
	{
		BlinkerController = gameObject.AddComponent<BlinkerController>();
		BlinkerController.BlinkColor = BlinkLoopParams[CurrentParamIndex].BlinkColor;
		BlinkerController.BlinkTransitionDuration = BlinkLoopParams[CurrentParamIndex].TransitionTimer;
		BlinkerController.BlinkDuration = BlinkLoopParams[CurrentParamIndex].BlinkTimer;
		yield return (StartCoroutine(BlinkerController.BlinkRoutine()));
		Destroy(BlinkerController);
		CurrentParamIndex = (CurrentParamIndex + 1) % NbParams;
		yield return (StartCoroutine(StartBlinker()));
	}
}
