using UnityEngine;
using TMPro;

public class PulseEffect : MonoBehaviour
{
    public static PulseEffect Instance;
    public TextMeshProUGUI timerText;
    private AudioSource audioSource;
    public AudioClip endAlertClip;
    public float pulseDuration = 0.3f;
    public float pulseScale = 1.2f;
    public Color pulseColor = Color.red;
    public Color originalColor;

    private Vector3 originalScale;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (timerText == null)
            timerText = GetComponent<TextMeshProUGUI>();

        originalScale = timerText.rectTransform.localScale;
        originalColor = timerText.color;

        audioSource = GetComponent<AudioSource>();
    }

    public void TriggerPulse()
    {
        StopAllCoroutines();
        StartCoroutine(Pulse());

        // Play beep if near end (assumes red = last 10 seconds)
        if (pulseColor == Color.red && audioSource != null)
            audioSource.Play();
    }

    private System.Collections.IEnumerator Pulse()
    {
        float t = 0f;
        Vector3 targetScale = originalScale * pulseScale;

        // Scale up
        while (t < pulseDuration / 2f)
        {
            t += Time.deltaTime;
            float factor = t / (pulseDuration / 2f);
            timerText.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, factor);
            timerText.color = Color.Lerp(originalColor, pulseColor, factor);
            yield return null;
        }

        t = 0f;

        // Scale back down
        while (t < pulseDuration / 2f)
        {
            t += Time.deltaTime;
            float factor = t / (pulseDuration / 2f);
            timerText.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, factor);
            timerText.color = Color.Lerp(pulseColor, originalColor, factor);
            yield return null;
        }

        timerText.rectTransform.localScale = originalScale;
        timerText.color = originalColor;
    }

    public void PlayEndAlert()
    {
        if (audioSource != null && endAlertClip != null)
            audioSource.PlayOneShot(endAlertClip);
    }
}