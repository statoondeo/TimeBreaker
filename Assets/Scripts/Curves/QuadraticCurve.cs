using UnityEngine;

public class QuadraticCurve : ICurve
{
	private Vector3 Point0;
	private Vector3 Point1;
	private Vector3 Point2;

	public QuadraticCurve(Vector3 point0, Vector3 point1, Vector3 point2)
	{
		Point0 = point0;
		Point1 = point1;
		Point2 = point2;
	}

	public Vector3 GetPoint(float t)
	{
		float newt = Mathf.Clamp01(t);
		float tReversed = 1 - newt;
		return (tReversed * tReversed * Point0 + 2 * newt * tReversed * Point1 + newt * newt * Point2);
	}
}
