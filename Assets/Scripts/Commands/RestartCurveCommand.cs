public class RestartCurveCommand : ICommand
{
	private readonly CurveFollower CurveFollower;

	public RestartCurveCommand(CurveFollower curveFollower) => CurveFollower = curveFollower;

	public void Execute() => CurveFollower.StartCurve();
}

