using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Clips de Música")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("Configuración Audio")]
    [Range(0f, 1f)]
    public float musicVolume = 0.6f;
    public bool adjustPitchWithTime = true;

    private AudioSource audioSource;
    private string currentSceneName = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = musicVolume;

        // Carga automática desde Resources si los clips no están asignados en el Inspector
        if (menuMusic == null)
        {
            menuMusic = Resources.Load<AudioClip>("Music/Menu Principal");
        }

        if (gameplayMusic == null)
        {
            gameplayMusic = Resources.Load<AudioClip>("Music/En el juego");
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        CheckAndPlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlayMusicForScene(scene.name);
    }

    public void CheckAndPlayMusicForScene(string sceneName)
    {
        if (audioSource == null) InitializeAudioSource();

        audioSource.pitch = 1.0f; // Reset pitch por defecto

        if (sceneName.Equals("Menu", System.StringComparison.OrdinalIgnoreCase))
        {
            PlayTrack(menuMusic);
        }
        else if (sceneName.Equals("Revision_carnet", System.StringComparison.OrdinalIgnoreCase))
        {
            PlayTrack(gameplayMusic);
        }
        else
        {
            // Para otras escenas, si es resumen de fin de día o similares
            if (audioSource.clip == null && gameplayMusic != null)
            {
                PlayTrack(gameplayMusic);
            }
        }

        currentSceneName = sceneName;
    }

    public void PlayTrack(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[MusicManager] Intentando reproducir un AudioClip nulo.");
            return;
        }

        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return; // Ya está sonando este tema
        }

        audioSource.clip = clip;
        audioSource.volume = musicVolume;
        audioSource.Play();
        Debug.Log($"[MusicManager] Reproduciendo tema: {clip.name}");
    }

    public void SetPitch(float pitch)
    {
        if (audioSource != null && adjustPitchWithTime)
        {
            audioSource.pitch = Mathf.Clamp(pitch, 0.8f, 1.3f);
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
