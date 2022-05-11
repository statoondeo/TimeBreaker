using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using static EventsService;
using static SoundService;
using static UnityEngine.ParticleSystem;

public class BallController : MonoBehaviour
{
	private static readonly List<Color> Colors = new List<Color>()
	{
		new Color(0.1019608f, 0.7372549f, 0.6117647f),
		new Color(0.2039216f, 0.5960785f, 0.8588236f),
		new Color(0.08627451f, 0.627451f, 0.5215687f),
		new Color(0.6078432f, 0.3490196f, 0.7137255f),
		new Color(0.1803922f, 0.8000001f, 0.4431373f),
		new Color(0.1529412f, 0.682353f, 0.3764706f),
		new Color(0.9450981f, 0.7686275f, 0.05882353f),
		new Color(0.1607843f, 0.5019608f, 0.7254902f),
		new Color(0.5568628f, 0.2666667f, 0.6784314f),
		new Color(0.9529412f, 0.6117647f, 0.07058824f),
		new Color(0.9019608f, 0.4941177f, 0.1333333f),
		new Color(0.8274511f, 0.3294118f, 0.0f),
		new Color(0.9058824f, 0.2980392f, 0.2352941f),
	};

	private static int CurrentColor = -1;
	private static Color GetnextColor()
	{
		CurrentColor = ++CurrentColor % Colors.Count;
		return (Colors[CurrentColor]);
	}

	[SerializeField] private Particles CollisionParticles;
	[SerializeField] private Particles DestroyParticles;
	[SerializeField] private bool Activated = true;
	[SerializeField] private Clips ExplosionSound;
	[SerializeField] private ParticleSystem TrailParticles;
	[SerializeField] private Light2D Light;

	private BallPhysicController BallPhysicController;
	private LineRenderer LineRenderer;
	private SpriteAnimator SpriteAnimator;

	public void Init(Color color)
	{
		LineRenderer.startColor = color;
		LineRenderer.endColor = color;

		Light.color = color;

		MainModule main = TrailParticles.main;
		main.startColor = color;

		SpriteAnimator.CurrentColor = color;
	}

	public void Predict(Vector3[] predictionPoints)
	{
		LineRenderer.enabled = false;
		if ((null == predictionPoints) || (predictionPoints.Length == 0)) return;

		LineRenderer.enabled = true;
		LineRenderer.positionCount = predictionPoints.Length;
		LineRenderer.SetPositions(predictionPoints);
	}

	private void Awake()
	{
		BallPhysicController = GetComponent<BallPhysicController>();
		LineRenderer = GetComponent<LineRenderer>();
		SpriteAnimator = GetComponent<SpriteAnimator>();

		if (CurrentColor == -1) CurrentColor = Random.Range(0, Colors.Count);
		Init(GetnextColor());
	}

	private void OnEnable()
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.Register(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void OnDisable()
{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnLevelStarted, OnLevelStartedCallback);
		eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
	}

	private void Start()
	{
		BallPhysicController.Activated = Activated;
		if (!Activated) return;
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnBallPopped, new OnBallPoppedEventArg() { Ball = this });
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!Activated) return;

		GameManager.Instance.GetService<ParticlesService>().Get(CollisionParticles, transform.position).Play();
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		OnBallCollidedEventArg evt = eventsService.GetOnBallCollidedEventArg();
		evt.Collision = collision;
		eventsService.Raise(Events.OnBallCollided, evt);
	}

	private void OnLevelEndedCallback(EventModelArg eventArg) => Kill(Random.value);

	private void OnLevelStartedCallback(EventModelArg eventArg)
	{
		Activated = true;
		BallPhysicController.Activated = Activated;
		GameManager.Instance.GetService<EventsService>().Raise(Events.OnBallPopped, new OnBallPoppedEventArg() { Ball = this });
	}

	public void Kill(float delay = 0.0f)
	{
		EventsService eventsService = GameManager.Instance.GetService<EventsService>();
		eventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
		eventsService.Raise(Events.OnBallKilled, new OnBallKilledEventArg() { Ball = this });
		StartCoroutine(KillBall(delay));
	}

	private IEnumerator KillBall(float delay)
	{
		if (0 != delay) yield return (new WaitForSeconds(delay));

		GameManager.Instance.GetService<SoundService>().Play(ExplosionSound);
		ParticleSystem particles = GameManager.Instance.GetService<ParticlesService>().Get(DestroyParticles, transform.position);
		particles.Play();
		gameObject.SetActive(false);
	}
}

