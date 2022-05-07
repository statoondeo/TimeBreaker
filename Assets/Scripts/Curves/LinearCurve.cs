using UnityEngine;

public class LinearCurve : ICurve
{
	private Vector3 Point0;
	private Vector3 Point1;

	public LinearCurve(Vector3 point0, Vector3 point1)
	{
		Point0 = point0;
		Point1 = point1;
	}

	public Vector3 GetPoint(float t)
	{
		float newt = Mathf.Clamp01(t);
		float tReversed = 1 - newt;
		return (Point0 * tReversed  + Point1 * newt);
	}
}
