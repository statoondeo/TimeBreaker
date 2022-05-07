using UnityEngine;

public class CubicCurve : ICurve
{
	private Vector3 Point0;
	private Vector3 Point1;
	private Vector3 Point2;
	private Vector3 Point3;

	public CubicCurve(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
	{
		Point0 = point0;
		Point1 = point1;
		Point2 = point2;
		Point3 = point3;
	}

	public Vector3 GetPoint(float t)
	{
		float newt = Mathf.Clamp01(t);
		float tReversed = 1 - newt;
		return (tReversed * tReversed * tReversed * Point0 + 3 * newt * tReversed * tReversed * Point1 + 3 * newt * newt * tReversed * Point2 + newt * newt * newt * Point3);
	}
}
