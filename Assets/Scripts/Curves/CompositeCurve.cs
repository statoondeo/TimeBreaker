using System.Collections.Generic;
using UnityEngine;

public class CompositeCurve : ICurve
{
	private readonly List<ICurve> Curves;
	private readonly int NbCurves;

	public CompositeCurve(params ICurve[] curves)
	{
		Curves = new List<ICurve>();
		if ((null != curves) && (0 != curves.Length))
		{
			NbCurves = curves.Length;
			for (int i = 0; i < NbCurves; i++)
			{
				Curves.Add(curves[i]);
			}
		}
	}

	public Vector3 GetPoint(float t)
	{
		float expandedt = t * NbCurves;
		int curveNumber = Mathf.Min(Mathf.FloorToInt(expandedt), NbCurves - 1);
		return (Curves[curveNumber].GetPoint(expandedt - curveNumber));
	}
}
