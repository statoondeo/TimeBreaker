using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static SoundService;

public class BadgeController : MonoBehaviour
{
    [SerializeField] private float WaitingValue = 0.0f;
    [SerializeField] private float InitialValue = 0.0f;
    [SerializeField] private float TargetValue = 1.0f;
    [SerializeField] private float Timer = 5.0f;
    [SerializeField] private Material ParticleMaterial;
    [SerializeField] private Sprite BadgeImage;
    [SerializeField] private Color ActiveColor = Color.green;
    [SerializeField] private Color InactiveColor = Color.gray;
    [SerializeField] private Image BadgeForeground;
    [SerializeField] private Image BadgeBackground;
    [SerializeField] private Tweening.Method TweeningMethod;
    [SerializeField] private Particles Particles;
    [SerializeField] private Clips RechargeSound;

    public event System.Action OnReady;
    public bool CanStart { get; private set; }

    private System.Func<float, float> TweeningFunction;
    private Coroutine Routine;

    private void Awake()
    {
        GameManager.Instance.EventsService.Register(EventsService.Events.OnLevelEnded, StopBadgeTimer);
        BadgeForeground.sprite = BadgeImage;
        BadgeForeground.color = ActiveColor;
        BadgeBackground.sprite = BadgeImage;
        BadgeBackground.color = InactiveColor;
        TweeningFunction = Tweening.GetMethod(TweeningMethod);
        Init();
    }

	private void OnEnable() => GameManager.Instance.EventsService.Register(EventsService.Events.OnLevelEnded, StopBadgeTimer);

	private void OnDisable() => GameManager.Instance.EventsService.UnRegister(EventsService.Events.OnLevelEnded, StopBadgeTimer);

    public void Init(bool canStart = true)
    {
        CanStart = canStart;
        BadgeForeground.fillAmount = WaitingValue;
    }

    private void StopBadgeTimer(EventModelArg eventArg)
    {
        if (null == Routine) return;
        GameManager.Instance.StopCoroutine(Routine);
    }

    public void StartBadgeTimer()
    {
        if (CanStart) Routine = StartCoroutine(BadgeTimerRoutine());
    }

    private IEnumerator BadgeTimerRoutine()
    {
        CanStart = false;
        float ttl = 0.0f;
        BadgeForeground.fillAmount = InitialValue;
        while (ttl < Timer)
        {
            BadgeForeground.fillAmount = Mathf.Lerp(InitialValue, TargetValue, TweeningFunction(ttl / Timer));
            yield return (null);
            ttl += Time.deltaTime;
        }
        BadgeForeground.fillAmount = TargetValue;
        GameManager.Instance.SoundService.Play(RechargeSound);
        GameManager.Instance.ParticlesService.Get(Particles, transform.position).Play();
        CanStart = true;
        OnReady?.Invoke();
    }
}
