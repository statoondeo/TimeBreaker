using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EventsService;

public class PredictionController : MonoBehaviour
{
    [SerializeField] private Transform PhysicObjectsContainer;
    [SerializeField] private float MaxDetectionIteration = 6.0f;
    [SerializeField] private float MaxPredictionIteration = 3.0f;
    [SerializeField] private float RefreshTime = 0.1f;

    private Tuple<Transform, PaddlePhysicController> PaddleController;

    private bool Activated;
    private bool ShieldRunning;

    private Scene CurrentScene;
    private PhysicsScene2D CurrentScenePhysic;

    private Scene PredictionScene;
    private PhysicsScene2D PredictionScenePhysic;

    private Dictionary<Rigidbody2D, Rigidbody2D> BallControllers;
    private Dictionary<Transform, Collider2D> BrickControllers;

    private OutOfBoundPhysicController OutOfBoundPhysicController;
    private Dictionary<Rigidbody2D, BallTrajectoryPrediction> BallTrajectoryPredictions;

    private void Awake()
	{
        BallControllers = new Dictionary<Rigidbody2D, Rigidbody2D>();
        BrickControllers = new Dictionary<Transform, Collider2D>();
        BallTrajectoryPredictions = new Dictionary<Rigidbody2D, BallTrajectoryPrediction>();
        ShieldRunning = false;
    }

	private void OnEnable()
	{
        EventsService eventsService = GameManager.Instance.GetService<EventsService>();
        eventsService.Register(Events.OnPlayerShieldStarted, OnPlayerShieldStartedCallback);
        eventsService.Register(Events.OnPlayerShieldEnded, OnPlayerShieldEndedCallback);
        eventsService.Register(Events.OnPaddlePopped, OnPaddlePoppedCallback);
        eventsService.Register(Events.OnBallPopped, OnBallPoppedCallback);
        eventsService.Register(Events.OnBallKilled, OnBallKilledCallback);
        eventsService.Register(Events.OnLevelStarted, OnlevelStartedCallback);
        eventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
    }

    private void OnDisable()
	{
        EventsService eventsService = GameManager.Instance.GetService<EventsService>();
        eventsService.UnRegister(Events.OnPlayerShieldStarted, OnPlayerShieldStartedCallback);
        eventsService.UnRegister(Events.OnPlayerShieldEnded, OnPlayerShieldEndedCallback);
        eventsService.UnRegister(Events.OnPaddlePopped, OnPaddlePoppedCallback);
        eventsService.UnRegister(Events.OnBallPopped, OnBallPoppedCallback);
        eventsService.UnRegister(Events.OnBallKilled, OnBallKilledCallback);
        eventsService.UnRegister(Events.OnLevelStarted, OnlevelStartedCallback);
        eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
        eventsService.UnRegister(Events.OnBrickPopped, OnBrickPoppedCallback);
        eventsService.UnRegister(Events.OnBrickKilled, OnBrickKilledCallback);
    }

    private void OnPlayerShieldStartedCallback(EventModelArg eventModelArg) => ShieldRunning = true;

    private void OnPlayerShieldEndedCallback(EventModelArg eventModelArg) => ShieldRunning = false;

    private void OnLevelEndedCallback(EventModelArg eventModelArg)
    {
        OnDisable();
        CancelInvoke(nameof(Predict));
    }

    private void OnBallKilledCallback(EventModelArg eventModelArg)
    {
        Rigidbody2D ballRigidbody2D = (eventModelArg as OnBallKilledEventArg).Ball.GetComponent<Rigidbody2D>();
        Destroy(BallControllers[ballRigidbody2D].gameObject);
        BallControllers.Remove(ballRigidbody2D);
        BallTrajectoryPredictions.Remove(ballRigidbody2D);
    }

    private void OnBrickKilledCallback(EventModelArg eventModelArg)
    {
        Transform brickTransform = (eventModelArg as OnBrickKilledEventArg).Brick.transform;
        if (!BrickControllers.ContainsKey(brickTransform)) return;
        Destroy(BrickControllers[brickTransform].gameObject);
        BrickControllers.Remove(brickTransform);
    }

    private GameObject GetGameObjectPhysicSkeleton(GameObject baseGameObject)
	{
        GameObject fakeGameObject = Instantiate(baseGameObject);
        Component[] components = fakeGameObject.GetComponents(typeof(Component));
        Component component;
        for (int j = 0, nbComponents = components.Length; j < nbComponents; j++)
        {
            component = components[j];
            if ((component is Collider2D) 
                || (component is Transform) 
                || (component is PaddlePhysicController) 
                || (component is BallPhysicController) 
                || (component is Rigidbody2D)) continue;
            Destroy(component);
        }
        for (int k = fakeGameObject.transform.childCount - 1; k >= 0; k--) Destroy(fakeGameObject.transform.GetChild(k).gameObject);
        return (fakeGameObject);
    }

    private void OnPaddlePoppedCallback(EventModelArg eventModelArg)
    {
        Transform paddleController = (eventModelArg as OnPaddlePoppedEventArg).PaddleController.transform;
        GameObject fakePaddle = GetGameObjectPhysicSkeleton(paddleController.gameObject);
        fakePaddle.SetActive(false);
        PaddleController = new Tuple<Transform, PaddlePhysicController>(paddleController, fakePaddle.GetComponent<PaddlePhysicController>());
    }

    private void OnBrickPoppedCallback(EventModelArg eventModelArg)
	{
        Transform brickController = (eventModelArg as OnBrickPoppedEventArg).Brick.transform;
        GameObject fakeObject = GetGameObjectPhysicSkeleton(brickController.gameObject);
        SceneManager.MoveGameObjectToScene(fakeObject, PredictionScene);
        BrickControllers.Add(brickController, fakeObject.GetComponent<Collider2D>());
    }

    private void OnBallPoppedCallback(EventModelArg eventModelArg)
    {
        Rigidbody2D ballController = (eventModelArg as OnBallPoppedEventArg).Ball.GetComponent<Rigidbody2D>();
        GameObject fakeObject = GetGameObjectPhysicSkeleton(ballController.gameObject);
        SceneManager.MoveGameObjectToScene(fakeObject, PredictionScene);
        Rigidbody2D fakeRigidBody = fakeObject.GetComponent<Rigidbody2D>();
        BallControllers.Add(ballController, fakeObject.GetComponent<Rigidbody2D>());
        BallTrajectoryPredictions.Add(ballController, new BallTrajectoryPrediction(fakeRigidBody, ballController, PaddleController.Item2, OutOfBoundPhysicController, MaxDetectionIteration, MaxPredictionIteration));
    }

    private void OnlevelStartedCallback(EventModelArg eventModelArg)
    {
		RebuildPredictionScene();
        PaddleController.Item2.gameObject.SetActive(true);
        PaddleController.Item2.Activated = true;
        SceneManager.MoveGameObjectToScene(PaddleController.Item2.gameObject, PredictionScene);
        EventsService eventsService = GameManager.Instance.GetService<EventsService>();
        eventsService.Register(Events.OnBrickPopped, OnBrickPoppedCallback);
        eventsService.Register(Events.OnBrickKilled, OnBrickKilledCallback);
        Activated = true;

        InvokeRepeating(nameof(Predict), RefreshTime, RefreshTime);
    }

    private void Start()
    {
        Activated = false;
		Physics2D.simulationMode = SimulationMode2D.Script;

		CurrentScene = SceneManager.GetActiveScene();
        CurrentScenePhysic = CurrentScene.GetPhysicsScene2D();

        PredictionScene = SceneManager.CreateScene("PredictionScene", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        PredictionScenePhysic = PredictionScene.GetPhysicsScene2D();        
    }

	private void FixedUpdate()
	{
		if (!Activated) return;

		CurrentScenePhysic.Simulate(Time.fixedDeltaTime);
	}

	private void Predict()
	{
        if (BallControllers.Count <= 0) return;

        Transform fakePaddle = PaddleController.Item2.transform;
        Transform realPaddle = PaddleController.Item1;

        fakePaddle.SetPositionAndRotation(realPaddle.position, realPaddle.rotation);

        PredictBalls();
    }

	private void RebuildPredictionScene()
	{
		DestroyAllPhysicObjects();
		CopyAllPhysicObjects();
	}

	private void CopyAllPhysicObjects()
	{
		Transform objectTransform;
		GameObject fakeObject;
		for (int i = 0, nbItems = PhysicObjectsContainer.childCount; i < nbItems; i++)
		{
			objectTransform = PhysicObjectsContainer.GetChild(i);
			if (!objectTransform.gameObject.TryGetComponent<Collider2D>(out _)) continue;

			fakeObject = Instantiate(objectTransform.gameObject, objectTransform.position, objectTransform.rotation);
			Component[] components = fakeObject.GetComponents(typeof(Component));
			Component component;
			for (int j = 0, nbComponents = components.Length; j < nbComponents; j++)
			{
				component = components[j];
                if ((component is Collider2D) || (component is Transform)) continue;
                if (component is OutOfBoundPhysicController) 
                {
                    OutOfBoundPhysicController = component as OutOfBoundPhysicController;
                    continue;
                }

                Destroy(component);
			}

			for (int k = fakeObject.transform.childCount - 1; k >= 0; k--) Destroy(fakeObject.transform.GetChild(k).gameObject);
            fakeObject.transform.SetPositionAndRotation(objectTransform.position, objectTransform.rotation);
			SceneManager.MoveGameObjectToScene(fakeObject, PredictionScene);
            BrickControllers.Add(objectTransform, fakeObject.GetComponent<Collider2D>());
        }
	}

	private void DestroyAllPhysicObjects()
	{
		foreach (Collider2D collider in BrickControllers.Values) Destroy(collider.gameObject);
		BrickControllers.Clear();
	}

    private void PredictBalls()
    {
        float timeSteps = Time.fixedDeltaTime;

        BallTrajectoryPrediction[] ballPredictions = new BallTrajectoryPrediction[BallTrajectoryPredictions.Values.Count];
		for (int i = 0, nbItems = ballPredictions.Length; i < nbItems; i++) ballPredictions[i] = BallTrajectoryPredictions.ElementAt(i).Value;

        int nbBallTrajectoryPrediction = ballPredictions.Length;
        for (int i = 0; i < nbBallTrajectoryPrediction; i++) ballPredictions[i].Init();

        bool oneBallActive = true;
        float duration = 3.0f;
        while(oneBallActive && (duration >= 0))
		{
            PredictionScenePhysic.Simulate(timeSteps);
            duration -= timeSteps;
            oneBallActive = false;
            for (int i = 0; i < nbBallTrajectoryPrediction; i++)
            {
                if (ballPredictions[i].IsActive()) ballPredictions[i].AddPosition(ShieldRunning);
                oneBallActive = oneBallActive || ballPredictions[i].IsActive();
            }
        }

        for (int i = 0; i < nbBallTrajectoryPrediction; i++) ballPredictions[i].Predict();
    }
}
