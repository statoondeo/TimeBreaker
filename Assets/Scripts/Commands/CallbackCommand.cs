using System;

public class CallbackCommand : ICommand
{
	private readonly Action Callback;

	public CallbackCommand(Action callback) => Callback = callback;

	public void Execute() => Callback.Invoke();
}