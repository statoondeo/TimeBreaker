using UnityEngine;

//[RequireComponent(typeof(LifeableBrickController))]
public class RegenBrickController : MonoBehaviour
{
	[SerializeField] private int Value;
	[SerializeField] private BadgeController BadgeController;

	private LifeableBrickController LifeableBrickController;

	private void Awake()
	{
		LifeableBrickController = GetComponent<LifeableBrickController>();
		BadgeController.OnReady += BadgeController_OnReady;
		BadgeController.Init();
	}

	private void BadgeController_OnReady()
	{
		BadgeController.Init();
		LifeableBrickController.AddLife(Value);
		if (LifeableBrickController.CanAddLife() && BadgeController.CanStart) BadgeController.StartBadgeTimer();
	}

	private void OnCollisionEnter2D()
	{
		if (LifeableBrickController.CanAddLife() && BadgeController.CanStart) BadgeController.StartBadgeTimer();
	}
}
