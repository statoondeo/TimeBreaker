public partial class EventsService
{
	public enum Events
	{
		OnSceneRequested = 0,
		OnLevelSetupEnded = 1,
		OnLevelSetupStarted = 2,
		OnBrickPopped = 3,
		OnBrickKilled = 4,
		OnLevelStarted = 5,
		OnLevelEnded = 6,
		OnBricksEnded = 7,
		OnTimerEnded = 8,
		OnBallKilled = 9,
		OnBallPopped = 10,
		OnBallsEnded = 11,
		OnBallCollided = 12,
		OnTimerPaused = 13,
		OnApplicationExit = 14,
		OnPaddlePopped = 15,
		OnTimerResume = 16,
		OnPlayerShieldStarted = 17,
		OnPlayerShieldEnded = 18,
	}
}
