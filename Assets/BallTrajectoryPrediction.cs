using System.Collections.Generic;
using UnityEngine;

public class BallTrajectoryPrediction
{
	private readonly PaddlePhysicController PaddlePhysicController;
	private readonly Rigidbody2D FakeRigidBody;
	private readonly Rigidbody2D RealRigidBody;
	private readonly List<Vector3> Positions;
	private readonly OutOfBoundPhysicController OutOfBoundPhysicController;
	private readonly float MaxDetection;
	private readonly float MaxPrediction;

	private bool Hitted;
	private bool Outted;
	private bool Active;
	private float CurrentLength;
	private bool DetectionMode;

	public BallTrajectoryPrediction(Rigidbody2D fakeRigidBody, Rigidbody2D realRigidBody, PaddlePhysicController paddlePhysicController, OutOfBoundPhysicController outOfBoundPhysicController, float maxDetection, float maxPrediction)
	{
		Positions = new List<Vector3>();
		FakeRigidBody = fakeRigidBody;
		RealRigidBody = realRigidBody;
		PaddlePhysicController = paddlePhysicController;
		OutOfBoundPhysicController = outOfBoundPhysicController;
		MaxDetection = maxDetection;
		MaxPrediction = maxPrediction;
	}

	public void Init()
	{
		Positions.Clear();

		FakeRigidBody.position = RealRigidBody.position;
		FakeRigidBody.velocity = RealRigidBody.velocity;
		CurrentLength = 0;

		Hitted = false;
		Active = true;
		Outted = false;
		DetectionMode = true;

		PaddlePhysicController.OnHit += OnHitCallback;
		OutOfBoundPhysicController.Hitted += OnOutOfBoundCallback;
	}

	private void OnOutOfBoundCallback()
	{
		Active = false;
		Outted = true;
		OutOfBoundPhysicController.Hitted -= OnOutOfBoundCallback;
	}

	private void OnHitCallback(Rigidbody2D rigidBody)
	{
		if (rigidBody.Equals(FakeRigidBody))
		{
			Hitted = true;
			PaddlePhysicController.OnHit -= OnHitCallback;
		}
	}

	public void AddPosition(bool shieldRunning)
	{
		Active = Active && !shieldRunning;
		if (!Active) return;

		Positions.Add(FakeRigidBody.position);
		CurrentLength += Time.fixedDeltaTime * FakeRigidBody.velocity.magnitude * 0.5f;
		if (DetectionMode)
		{
			if (!Hitted && !Outted && ((CurrentLength >= MaxDetection) || (FakeRigidBody.position.y >= 1.0f) || (FakeRigidBody.velocity.y >= 0.0f)))
			{
				Active = false;
				return;
			}
			DetectionMode = Hitted;
			CurrentLength = 0.0f;
		}
		else
		{
			Active = CurrentLength < MaxPrediction;
		}
	}

	public void Predict()
	{
		PaddlePhysicController.OnHit -= OnHitCallback;
		OutOfBoundPhysicController.Hitted -= OnOutOfBoundCallback;
		RealRigidBody.GetComponent<BallController>().Predict(Hitted || Outted ? Positions.ToArray() : null);
	}

	public bool IsActive() => Active;
}
