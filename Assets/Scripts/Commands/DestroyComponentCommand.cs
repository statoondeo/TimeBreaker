using UnityEngine;

public class DestroyComponentCommand : ICommand
{
	private readonly Component ComponentToDestroy;

	public DestroyComponentCommand(Component componentToDestroy) => ComponentToDestroy = componentToDestroy;

	public void Execute() => GameObject.Destroy(ComponentToDestroy);
}

