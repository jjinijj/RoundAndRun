using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSourceA;
    [SerializeField] private AudioSource bgmSourceB;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Settings")]
    [SerializeField] private float crossfadeDuration = 1.5f;

    [Header("SFX Sound")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip slideClip;
    [SerializeField] private AudioClip hitClip;

    private AudioSource activeSource;
    private AudioSource inactiveSource;
    private Coroutine crossfadeCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        activeSource = bgmSourceA;
        inactiveSource = bgmSourceB;
    }

    public void PlayItemPickup(AudioClip audioClip) => sfxSource.PlayOneShot(audioClip);
    public void PlayJump() => sfxSource.PlayOneShot(jumpClip);
    public void PlaySlide() => sfxSource.PlayOneShot(slideClip);
    public void PlayHit() => sfxSource.PlayOneShot(hitClip);

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (activeSource.clip == clip && activeSource.isPlaying) return;

        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);

        inactiveSource.clip = clip;
        inactiveSource.loop = loop;
        inactiveSource.volume = 0f;
        inactiveSource.Play();

        crossfadeCoroutine = StartCoroutine(Crossfade());
    }

    IEnumerator Crossfade()
    {
        float elapsed = 0f;
        float startVolume = activeSource.volume;

        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / crossfadeDuration;
            activeSource.volume = Mathf.Lerp(startVolume, 0f, t);
            inactiveSource.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = 1f;

        (activeSource, inactiveSource) = (inactiveSource, activeSource);
        crossfadeCoroutine = null;
    }

    public void StopBGM()
    {
        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        activeSource.Stop();
        inactiveSource.Stop();
    }

    public void PauseBGM()
    {
        activeSource.Pause();
        inactiveSource.Pause();
    }

    public void ResumeBGM()
    {
        activeSource.UnPause();
        if (crossfadeCoroutine != null) inactiveSource.UnPause();
    }
}
