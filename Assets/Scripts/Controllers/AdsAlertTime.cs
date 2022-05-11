using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdsAlertTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TimerText;
    [SerializeField] private Image AlertIcon;

    void Update()
    {
        float nextAdTime = GameManager.Instance.GetService<AdsService>().NextAdTime;
        if (Time.realtimeSinceStartup >= nextAdTime)
		{
            TimerText.gameObject.SetActive(false);
            AlertIcon.gameObject.SetActive(true);
        }
        else
		{
            AlertIcon.gameObject.SetActive(false);
            TimerText.gameObject.SetActive(true);
            TimeSpan time = TimeSpan.FromSeconds(nextAdTime - Time.realtimeSinceStartup);
            TimerText.text = time.ToString(@"m\:ss");
        }
    }
}
