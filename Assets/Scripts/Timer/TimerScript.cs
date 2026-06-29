using UnityEngine;
using TMPro;
using System;

public class GameClock : MonoBehaviour
{
    public static event Action OnGameStarted;
    public static event Action<int> OnTurnoChanged;
    public static event Action<int, int> OnTimeChanged;

    public static int TurnoGlobal = 0;

    public TextMeshProUGUI clockText;

    public int HoraInicio = 12;
    public int MinutoInicio = 0;

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

        ActualizarTurnoGlobal();
        UpdateClockText();

        OnTimeChanged?.Invoke(HoraActual, MinutoActual);
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
            ActualizarTurnoGlobal();

            OnTimeChanged?.Invoke(HoraActual, MinutoActual);

            CheckGameStart();

            if (HasReachedEndTime())
            {
                RelojActivo = false;
                Debug.Log("Fin del juego");
                if (DayManager.instance != null)
                {
                    DayManager.instance.EndDay();
                }
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

    void ActualizarTurnoGlobal()
    {
        int nuevoTurno = CalcularTurnoActual();

        if (nuevoTurno != TurnoGlobal)
        {
            TurnoGlobal = nuevoTurno;

            Debug.Log("Turno global actual: " + TurnoGlobal);

            OnTurnoChanged?.Invoke(TurnoGlobal);
        }
    }

    int CalcularTurnoActual()
    {
        int minutosActuales = HoraActual * 60 + MinutoActual;
        int minutosInicio = 12 * 60;
        int minutosFinal = 14 * 60 + 30;

        if (minutosActuales < minutosInicio)
        {
            return 0;
        }

        if (minutosActuales >= minutosFinal)
        {
            return 10;
        }

        int minutosDesdeInicio = minutosActuales - minutosInicio;

        return (minutosDesdeInicio / 15) + 1;
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