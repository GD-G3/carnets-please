using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager instance;

    [Header("Efectos de Sonido")]
    public AudioClip buttonClickClip;
    [Range(0f, 1f)]
    public float soundVolume = 0.8f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.volume = soundVolume;

        if (buttonClickClip == null)
        {
            buttonClickClip = Resources.Load<AudioClip>("SFX/button_click");
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
        RegisterAllSceneButtons();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterAllSceneButtons();
    }

    public void RegisterAllSceneButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (Button btn in buttons)
        {
            btn.onClick.RemoveListener(PlayButtonClickSound);
            btn.onClick.AddListener(PlayButtonClickSound);
        }
    }

    public void PlayButtonClickSound()
    {
        if (audioSource == null) InitializeAudio();

        if (buttonClickClip == null)
        {
            buttonClickClip = Resources.Load<AudioClip>("SFX/button_click");
        }

        if (buttonClickClip != null)
        {
            audioSource.PlayOneShot(buttonClickClip, soundVolume);
        }
    }
}
