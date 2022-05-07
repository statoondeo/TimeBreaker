using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static EventsService;

public class CumulativedBadgeController : MonoBehaviour
{
    [SerializeField] private float WaitingValue = 0.0f;
    [SerializeField] private float InitialValue = 0.0f;
    [SerializeField] private float TargetValue = 1.0f;
    [SerializeField] private Material ParticleMaterial;
    [SerializeField] private Sprite BadgeImage;
    [SerializeField] private Color ActiveColor = Color.yellow;
    [SerializeField] private Color InactiveColor = Color.gray;
    [SerializeField] private Image BadgeForeground;
    [SerializeField] private Image BadgeBackground;
    [SerializeField] private Tweening.Method TweeningMethod;
    [SerializeField] private Particles Particles;
    [SerializeField] private Events StartEvent;
    [SerializeField] private Events StopEvent;
    [SerializeField] private GameObject ObjectToPop;

    private System.Func<float, float> TweeningFunction;
    private Coroutine Routine;
    private bool Running = false;
    private float Timer;

    private void Awake()
    {
        BadgeForeground.sprite = BadgeImage;
        BadgeForeground.color = ActiveColor;
        BadgeBackground.sprite = BadgeImage;
        BadgeBackground.color = InactiveColor;
        TweeningFunction = Tweening.GetMethod(TweeningMethod);
        BadgeForeground.fillAmount = WaitingValue;
    }

	private void OnEnable()
	{
        GameManager.Instance.EventsService.Register(StartEvent, OnStartEventTriggeredCallback);
        GameManager.Instance.EventsService.Register(Events.OnLevelEnded, OnLevelEndedCallback);
    }

    private void OnDisable()
    {
        GameManager.Instance.EventsService.UnRegister(StartEvent, OnStartEventTriggeredCallback);
        GameManager.Instance.EventsService.UnRegister(Events.OnLevelEnded, OnLevelEndedCallback);
    }

    private void OnLevelEndedCallback(EventModelArg eventArg)
	{
        if (null == Routine) return;
        GameManager.Instance.StopCoroutine(Routine);
    }

    private void OnStartEventTriggeredCallback(EventModelArg eventArg)
    {
        float duration = 0.0f;
        if (eventArg is OnTimerPausedEventArg) duration = (eventArg as OnTimerPausedEventArg).Duration;
        if (eventArg is OnPlayerShieldStartedEventArg) duration = (eventArg as OnPlayerShieldStartedEventArg).Duration;
        if (Running)
        {
            Timer += duration;
        }
        else
        {
            Timer = duration;
            Routine = StartCoroutine(BadgeTimerRoutine());
            if (eventArg is OnPlayerShieldStartedEventArg) Instantiate(ObjectToPop);
        }
        GameManager.Instance.ParticlesService.Get(Particles, transform.position).Play();
    }

    private IEnumerator BadgeTimerRoutine()
    {
        Running = true;
        float ttl = 0.0f;
        BadgeForeground.fillAmount = InitialValue;
        while (ttl < Timer)
        {
            BadgeForeground.fillAmount = Mathf.Lerp(InitialValue, TargetValue, TweeningFunction(ttl / Timer));
            yield return (null);
            ttl += Time.deltaTime;
        }
        BadgeForeground.fillAmount = TargetValue;
        BadgeForeground.fillAmount = WaitingValue;
        Running = false;
        GameManager.Instance.EventsService.Raise(StopEvent);
    }
}
