using UnityEngine;

public class SplineDrawer : MonoBehaviour
{
	private ControlPoint[] ControlPoints;

	private void Awake()
	{
		ControlPoints = GetControlPoints();
	}

	private ControlPoint[] GetControlPoints()
	{
		return (GetComponentsInChildren<ControlPoint>());
	}

	public void SetStartPosition(Vector3 position)
	{
		ControlPoints[0].transform.position = position;
	}
	public void SetStartRotation(Quaternion quaternion)
	{
		ControlPoints[0].transform.rotation = quaternion;
	}
	public void SetEndPosition(Vector3 position)
	{
		ControlPoints[ControlPoints.Length - 1].transform.position = position;
	}
	public void SetEndRotation(Quaternion quaternion)
	{
		ControlPoints[ControlPoints.Length - 1].transform.rotation = quaternion;
	}

	public ICurve GetCurve()
	{
		if (ControlPoints.Length == 2) return (new CubicCurve(ControlPoints[0].CheckPoint, ControlPoints[0].ControlPointB, ControlPoints[1].ControlPointA, ControlPoints[1].CheckPoint));

		ICurve[] curves = new CubicCurve[ControlPoints.Length - 1];
		for (int i = 1, nbItems = ControlPoints.Length; i < nbItems; i++)
		{
			curves[i - 1] = new CubicCurve(ControlPoints[i - 1].CheckPoint, ControlPoints[i - 1].ControlPointB, ControlPoints[i].ControlPointA, ControlPoints[i].CheckPoint);
		}

		return (new CompositeCurve(curves));
	}

	#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
		const float curveStep = 0.05f;

		Gizmos.color = Color.green;
		ControlPoint[] controlPoints = GetControlPoints();
		for (int i = 1, nbItems = controlPoints.Length; i < nbItems; i++)
		{
			ICurve curve = new CubicCurve(controlPoints[i - 1].CheckPoint, controlPoints[i - 1].ControlPointB, controlPoints[i].ControlPointA, controlPoints[i].CheckPoint);
			Vector3 previousPoint = controlPoints[i - 1].CheckPoint;
			for (float t = curveStep; t <= 1.0f + curveStep; t += curveStep)
			{
				Vector3 nextPoint = curve.GetPoint(t);
				Gizmos.DrawLine(previousPoint, nextPoint);
				previousPoint = nextPoint;
			}
		}
	}

	#endif
}
