using UnityEngine;
using TMPro;

public class VisualIntensityManager : MonoBehaviour
{
    public static VisualIntensityManager instance;

    [Header("Referencia")]
    public TextMeshProUGUI clockText;

    [Header("Intensidad")]
    public Color colorNormal = new Color(0.9f, 0.95f, 1f, 1f);
    public Color colorMedio = new Color(1f, 0.85f, 0.2f, 1f);
    public Color colorIntenso = new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Efectos")]
    public bool animarTextoReloj = true;
    public float escalaPulsoTick = 1.15f;
    public float velocidadRetornoEscala = 5f;

    private Vector3 escalaOriginalTexto = Vector3.one;
    private Color colorObjetivoReloj;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameClock.OnTimeChanged += OnTimeChanged;
    }

    private void OnDisable()
    {
        GameClock.OnTimeChanged -= OnTimeChanged;
    }

    private void Start()
    {
        if (clockText != null)
        {
            escalaOriginalTexto = clockText.transform.localScale;
            colorObjetivoReloj = colorNormal;
            clockText.color = colorNormal;
        }
        else
        {
            // Intentar buscar el texto del reloj en la escena
            GameClock clock = FindAnyObjectByType<GameClock>();
            if (clock != null && clock.clockText != null)
            {
                clockText = clock.clockText;
                escalaOriginalTexto = clockText.transform.localScale;
                colorObjetivoReloj = colorNormal;
            }
        }
    }

    private void Update()
    {
        if (clockText == null) return;

        if (animarTextoReloj && clockText.transform.localScale != escalaOriginalTexto)
        {
            clockText.transform.localScale = Vector3.Lerp(
                clockText.transform.localScale,
                escalaOriginalTexto,
                Time.deltaTime * velocidadRetornoEscala
            );
        }

        // Parpadeo suave si el color es muy intenso (rojo)
        if (colorObjetivoReloj == colorIntenso)
        {
            float alphaPulse = Mathf.PingPong(Time.time * 3f, 0.4f) + 0.6f;
            Color colorConPulso = colorIntenso;
            colorConPulso.a = alphaPulse;
            clockText.color = Color.Lerp(clockText.color, colorConPulso, Time.deltaTime * 8f);
        }
        else
        {
            clockText.color = Color.Lerp(clockText.color, colorObjetivoReloj, Time.deltaTime * 4f);
        }
    }

    private void OnTimeChanged(int hora, int minuto)
    {
        // Calcular minutos totales desde inicio
        int minutosTotales = hora * 60 + minuto;
        int inicioMinutos = 12 * 60; // 720
        int finalMinutos = 14 * 60 + 30; // 870

        float progresoJornada = Mathf.Clamp01((float)(minutosTotales - inicioMinutos) / (finalMinutos - inicioMinutos));

        // Actualizar color e intensidad
        if (progresoJornada < 0.4f)
        {
            colorObjetivoReloj = colorNormal;
        }
        else if (progresoJornada < 0.75f)
        {
            float t = (progresoJornada - 0.4f) / 0.35f;
            colorObjetivoReloj = Color.Lerp(colorNormal, colorMedio, t);
        }
        else
        {
            float t = (progresoJornada - 0.75f) / 0.25f;
            colorObjetivoReloj = Color.Lerp(colorMedio, colorIntenso, t);
        }

        // Animar pulso visual en el texto del reloj con cada tick de tiempo
        if (animarTextoReloj && clockText != null)
        {
            float multiplicadorIntensidad = 1f + (progresoJornada * 0.15f);
            clockText.transform.localScale = escalaOriginalTexto * (escalaPulsoTick * multiplicadorIntensidad);
        }

        // Ajustar ligeramente el pitch de la música con la aceleración final
        if (MusicManager.instance != null)
        {
            float pitchPitching = 1.0f + (progresoJornada * 0.12f);
            MusicManager.instance.SetPitch(pitchPitching);
        }
    }
}
