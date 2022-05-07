using UnityEngine;
using static EventsService;

/// <summary>
/// Classe définissant la phase de démarrage du jeu
/// </summary>
public class BootStrapper
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnRuntimeMethodLoad()
	{
		Application.targetFrameRate = 60;

		// Gestion du jeu
		GameManager.Instantiate();

		// On démarre le jeu par le menu
		GameManager.Instance.EventsService.Raise(Events.OnSceneRequested, new OnSceneRequestedEventArg() { Scene = SceneNames.Menu });
	}
}
