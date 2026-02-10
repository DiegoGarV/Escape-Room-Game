using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip victoryMusic;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip bookPickupSfx;
    [SerializeField] private AudioClip keyPickupSfx;
    [SerializeField] private AudioClip pillowPickupSfx;
    [SerializeField] private AudioClip woodDoorOpenSfx;
    [SerializeField] private AudioClip metalDoorOpenSfx;
    [SerializeField] private AudioClip firePickupSfx;
    [SerializeField] private AudioClip[] footstepClips;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float musicVolume = 0.6f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 0.9f;
    [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.8f;
    [SerializeField] private float footstepMinPitch = 0.95f;
    [SerializeField] private float footstepMaxPitch = 1.05f;

    [Header("3D Settings")]
    [SerializeField, Range(0f, 1f)] private float sfx3dVolume = 1f;
    [SerializeField] private float minDistance = 1.5f;
    [SerializeField] private float maxDistance = 20f;

    private AudioSource _musicSource;
    private Coroutine _fadeRoutine;
    private AudioSource _sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume = musicVolume;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip target = null;

        if (sceneName == "MainMenu")
            target = menuMusic;
        else if (sceneName == "EscapeRoom")
            target = gameMusic;

        if (target == null) return;
        FadeToClip(target);
    }

    public void PlayVictoryMusic()
    {
        if (victoryMusic == null) return;
        FadeToClip(victoryMusic);
    }

    void EnsureSfxSource()
    {
        if (_sfxSource != null) return;
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;
        _sfxSource.volume = sfxVolume;
    }

    public void PlayPickupSfx(CollectableType type)
    {
        EnsureSfxSource();

        AudioClip clip = type switch
        {
            CollectableType.BlueBook => bookPickupSfx,
            CollectableType.RedBook => bookPickupSfx,
            CollectableType.GreenBook => bookPickupSfx,
            CollectableType.BoringBook => bookPickupSfx,
            CollectableType.Key => keyPickupSfx,
            CollectableType.Pillow => pillowPickupSfx,
            _ => null
        };

        if (clip != null)
            _sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayDoorOpenSfx3D(DoorType type, Vector3 worldPos)
    {
        AudioClip clip = type switch
        {
            DoorType.Wood => woodDoorOpenSfx,
            DoorType.Metal => metalDoorOpenSfx,
            _ => null
        };

        if (clip == null) return;

        var go = new GameObject($"SFX_DoorOpen_{type}");
        go.transform.position = worldPos;

        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = sfx3dVolume;

        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;

        src.playOnAwake = false;
        src.loop = false;

        src.Play();
        Destroy(go, clip.length + 0.1f);
    }

    public void PlayFirePickupSfx(Vector3 worldPos)
    {
        if (firePickupSfx == null) return;

        var go = new GameObject("SFX_FirePickup");
        go.transform.position = worldPos;

        var src = go.AddComponent<AudioSource>();
        src.clip = firePickupSfx;
        src.volume = sfx3dVolume;

        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = minDistance;
        src.maxDistance = maxDistance;

        src.playOnAwake = false;
        src.loop = false;

        src.Play();
        Destroy(go, firePickupSfx.length + 0.1f);
    }

    public void PlayFootstep()
    {
        EnsureSfxSource();

        if (footstepClips == null || footstepClips.Length == 0) return;

        var clip = footstepClips[Random.Range(0, footstepClips.Length)];
        if (clip == null) return;

        _sfxSource.pitch = Random.Range(footstepMinPitch, footstepMaxPitch);
        _sfxSource.PlayOneShot(clip, footstepVolume);
    }

    private void FadeToClip(AudioClip newClip)
    {
        if (_musicSource.clip == newClip && _musicSource.isPlaying) return;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeTo(newClip));
    }

    private IEnumerator FadeTo(AudioClip newClip)
    {
        float startVol = _musicSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            _musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
            yield return null;
        }

        _musicSource.volume = 0f;
        _musicSource.clip = newClip;
        _musicSource.Play();

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            _musicSource.volume = Mathf.Lerp(0f, musicVolume, t / fadeDuration);
            yield return null;
        }

        _musicSource.volume = musicVolume;
        _fadeRoutine = null;
    }

    public void SetMusicVolume(float v)
    {
        musicVolume = Mathf.Clamp01(v);
        _musicSource.volume = musicVolume;
    }
}
