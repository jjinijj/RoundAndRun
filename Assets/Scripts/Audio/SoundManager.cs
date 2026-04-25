using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Sound")]
    [SerializeField] private AudioClip bgm;

    [Header("SFX Sound")]

    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip slideClip;
    [SerializeField] private AudioClip hitClip;


    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void PlayItemPickup(AudioClip audioClip) => sfxSource.PlayOneShot(audioClip);
    public void PlayJump() => sfxSource.PlayOneShot(jumpClip);
    public void PlaySlide() => sfxSource.PlayOneShot(slideClip);
    public void PlayHit() => sfxSource.PlayOneShot(hitClip);

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();
}
