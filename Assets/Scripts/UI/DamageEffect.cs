using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private Image vignetteImage;
    [SerializeField] private float flashDuration = 0.3f;

    private Coroutine currentCoroutine;

    void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        SetAlpha(0.0f);
    }
    public void PlayEffect()
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // 빠르게 올라오고
        float elapsed = 0f;
        float halfDuration = flashDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0f, 0.8f, elapsed / halfDuration));
            yield return null;
        }

        // 천천히 사라지고
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(0.8f, 0f, elapsed / halfDuration));
            yield return null;
        }

        SetAlpha(0f);
    }

    void SetAlpha(float alpha)
    {
        Color c = vignetteImage.color;
        c.a = alpha;
        vignetteImage.color = c;
    }
}