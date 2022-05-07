using System;
using UnityEngine;

[Serializable]
public class SetupCurveParameter
{
	public bool HasParameter;
	public Vector3 InitialtPosition;
	public Quaternion InitialRotation;
	public Vector3 TargetPosition;
	public Quaternion TargetRotation;
	public float Timer;
	public float Delay;
	public int Order;
	public Tweening.Method Tweening;
	public SplineDrawer Spline;
}


