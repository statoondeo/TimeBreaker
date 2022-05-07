using UnityEngine;
using static EventsService;

/// <summary>
/// Classe d�finissant la phase de d�marrage du jeu
/// </summary>
public class BootStrapper
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnRuntimeMethodLoad()
	{
		Application.targetFrameRate = 60;

		// Gestion du jeu
		GameManager.Instantiate();

		// On d�marre le jeu par le menu
		GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });
	}
}
