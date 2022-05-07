using System;
using UnityEngine;

[Serializable]
public class BrickInLevelModel
{
	[Header("Brick")]
	public string BrickPrefab;
	[Header("Setup")]
	public SetupCurveParameter SetupCurve;
	[Header("Behaviour")]
	public BehaviourCurveParameter BehaviourCurve;
}