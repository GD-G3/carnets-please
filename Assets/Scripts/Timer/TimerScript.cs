using UnityEngine;
using TMPro;
using System;

public class GameClock : MonoBehaviour
{
    public static event Action OnGameStarted;
    public TextMeshProUGUI clockText;

    public int HoraInicio = 11;
    public int MinutoInicio = 55;

    public int HoraFinal = 14;
    public int MinutoFinal = 30;

    public float SegundosporTick = 3f;
    
    public int minMinutesPerTick = 1;
    public int maxMinutesPerTick = 3;
    
    private int HoraActual;
    private int MinutoActual;

    private float timer;
    private bool RelojActivo = true;

    private bool JuegoEmpezo = false;

    void Start()
    {
        HoraActual = HoraInicio;
        MinutoActual = MinutoInicio;

        UpdateClockText();
    }

    void Update()
    {
        if (!RelojActivo) return;

        timer += Time.deltaTime;

        if (timer >= SegundosporTick)
        {
            timer = 0f;

            int MinutosdelJuegoporTick = UnityEngine.Random.Range(minMinutesPerTick, maxMinutesPerTick + 1);

            AddGameMinutes(MinutosdelJuegoporTick);
            UpdateClockText();

            CheckGameStart();

            if (HasReachedEndTime())
            {
                RelojActivo = false;
                Debug.Log("Fin del juego");
            }
        }
    }

void CheckGameStart()
    {
        if (JuegoEmpezo) return;

        if (HoraActual >= 12)
        {
            JuegoEmpezo = true;
            Debug.Log("Son las 12. Empiezan a pasar los alumnos");

            OnGameStarted?.Invoke();
        }
    }

    void AddGameMinutes(int minutes)
    {
        MinutoActual += minutes;

        while (MinutoActual >= 60)
        {
            MinutoActual -= 60;
            HoraActual++;
        }
    }

    bool HasReachedEndTime()
    {
        if (HoraActual > HoraFinal) return true;
        if (HoraActual == HoraFinal && MinutoActual >= MinutoFinal) return true;

        return false;
    }

    void UpdateClockText()
    {
        string period = HoraActual >= 12 ? "PM" : "AM";

        int displayHour = HoraActual;
        if (displayHour > 12) displayHour -= 12;

        clockText.text = displayHour.ToString("00") + ":" + MinutoActual.ToString("00") + " " + period;
    }
}