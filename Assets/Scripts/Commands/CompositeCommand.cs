using System.Collections.Generic;

public class CompositeCommand : ICommand
{
	private readonly List<ICommand> Commands;

	public CompositeCommand(List<ICommand> commands) => Commands = commands;

	public void Execute()
	{
		for (int i = 0, nbItems = Commands.Count; i < nbItems; i++) Commands[i].Execute();
	}
}
