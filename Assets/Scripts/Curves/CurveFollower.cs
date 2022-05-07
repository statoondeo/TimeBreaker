using System;
using System.Collections;
using UnityEngine;

public class CurveFollower : MonoBehaviour
{
	public float Timer;
	public float StartingTimer = 0.0f;
	public float Delay = 0.0f;
	public int Order = 0;
	public Func<float, float> TweeningMethod;
	public SplineDrawer SplineDrawer;
	public GameObject Target;
	public ICommand OnDoneCommand;

	private ICurve Curve;
	private WaitForSeconds WaitForSeconds;
	public void StartCurve()
	{
		StartCurve(0.0f);
	}

	public void StartCurve(float starting)
	{
		WaitForSeconds = new WaitForSeconds(Delay * Order);
		Curve = SplineDrawer.GetCurve();
		StartCoroutine(CurveFollowingRoutine(starting));
	}

	public static CurveFollower AddCurveFollower(GameObject newGameObject, SetupCurveParameter setupCurveParameter)
	{
		if (!setupCurveParameter.HasParameter) return (null);
		CurveFollower curveFollower = newGameObject.AddComponent<CurveFollower>();
		curveFollower.SplineDrawer = Instantiate(setupCurveParameter.Spline, newGameObject.transform);
		curveFollower.SplineDrawer.SetStartPosition(setupCurveParameter.InitialtPosition);
		curveFollower.SplineDrawer.SetStartRotation(setupCurveParameter.InitialRotation);
		curveFollower.SplineDrawer.SetEndPosition(setupCurveParameter.TargetPosition);
		curveFollower.SplineDrawer.SetEndRotation(setupCurveParameter.TargetRotation);
		curveFollower.Timer = setupCurveParameter.Timer;
		curveFollower.Delay = setupCurveParameter.Delay;
		curveFollower.Order = setupCurveParameter.Order;
		curveFollower.TweeningMethod = Tweening.GetMethod(setupCurveParameter.Tweening);
		curveFollower.Target = newGameObject;
		return (curveFollower);
	}

	public static CurveFollower AddCurveFollower(GameObject newGameObject, BehaviourCurveParameter behaviourCurveParameter)
	{
		if (!behaviourCurveParameter.HasParameter) return (null);
		CurveFollower curveFollower = newGameObject.AddComponent<CurveFollower>();
		curveFollower.SplineDrawer = Instantiate(behaviourCurveParameter.Spline);
		curveFollower.Timer = behaviourCurveParameter.Timer;
		curveFollower.StartingTimer = behaviourCurveParameter.Start;
		curveFollower.Delay = 0.0f;
		curveFollower.Order = 0;
		curveFollower.TweeningMethod = Tweening.GetMethod(behaviourCurveParameter.Tweening);
		curveFollower.Target = newGameObject;
		return (curveFollower);
	}

	private IEnumerator CurveFollowingRoutine(float starting)
	{
		Target.transform.position = Curve.GetPoint(0.0f);
		if ((Delay != 0) && (Order != 0))
		{
			yield return (WaitForSeconds);
		}
		float ttl = starting * Timer;
		while (ttl < Timer)
		{
			Target.transform.position = Curve.GetPoint(TweeningMethod(ttl / Timer));
			yield return (null);
			ttl += Time.deltaTime;
		}
		Target.transform.position = Curve.GetPoint(1.0f);
		OnDoneCommand?.Execute();
	}
}
