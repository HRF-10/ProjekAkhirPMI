using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Musik")]
    public AudioClip music;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Suara")]
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public AudioClip jump;
    public AudioClip slide;
    public AudioClip flip;
    public AudioClip coin;
    public AudioClip hit;
    public AudioClip qteSuccess;
    public AudioClip qteFail;
    public AudioClip gameOver;

    AudioSource musicSource;
    AudioSource sfxSource;
    int lastLives = -1;

    void Awake()
    {
        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;
        musicSource.clip = music;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        if (music != null) musicSource.Play();
    }

    void OnEnable()
    {
        QTEManager.OnQTESuccess += PlayQteSuccess;
        QTEManager.OnQTEFailed += PlayQteFail;
        LivesManager.OnLivesChanged += OnLivesChanged;
    }

    void OnDisable()
    {
        QTEManager.OnQTESuccess -= PlayQteSuccess;
        QTEManager.OnQTEFailed -= PlayQteFail;
        LivesManager.OnLivesChanged -= OnLivesChanged;
    }

    void OnLivesChanged(int lives)
    {
        if (lastLives >= 0 && lives < lastLives) PlayHit();
        lastLives = lives;
    }

    void Play(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public static void PlayJump() { if (Instance != null) Instance.Play(Instance.jump); }
    public static void PlaySlide() { if (Instance != null) Instance.Play(Instance.slide); }
    public static void PlayFlip() { if (Instance != null) Instance.Play(Instance.flip); }
    public static void PlayCoin() { if (Instance != null) Instance.Play(Instance.coin); }
    public static void PlayHit() { if (Instance != null) Instance.Play(Instance.hit); }
    public static void PlayQteSuccess() { if (Instance != null) Instance.Play(Instance.qteSuccess); }
    public static void PlayQteFail() { if (Instance != null) Instance.Play(Instance.qteFail); }

    public static void PlayGameOver()
    {
        if (Instance == null) return;
        Instance.musicSource.Stop();
        Instance.Play(Instance.gameOver);
    }
}