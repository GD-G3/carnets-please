using UnityEngine;
using System;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

    [Header("Progreso del Juego")]
    public int diaActual = 1;
    public int diaMaximo = 5;

    [Header("Referencias")]
    public QueueSystem queueSystem;
    public CarnetGenerator carnetGenerator;
    public CameraThreatManager cameraThreatManager;

    public event Action<int> OnDayStarted;
    public event Action OnAllDaysCompleted;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        cameraThreatManager = FindAnyObjectByType<CameraThreatManager>();
        queueSystem = UnityEngine.Object.FindAnyObjectByType<QueueSystem>();
        carnetGenerator = UnityEngine.Object.FindAnyObjectByType<CarnetGenerator>();
        StartDay();
    }

    private void Start()
    {
        // Si hay una escena ya cargada sin pasar por sceneLoaded, arrancamos el día
        // (Aunque sceneLoaded también puede dispararse. Usamos un flag para evitar doble ejecución si hace falta, pero StartDay es seguro de llamar)
        // Ya se llama StartDay() en OnSceneLoaded, pero Unity a veces dispara Start después.
        // Mejor llamarlo solo si es el inicio absoluto (día 1 recién arranca y no viene de un reload)
        if (diaActual == 1)
        {
            StartDay();
        }
    }

    private bool diaIniciado = false;

    public void StartDay()
    {
        if (diaIniciado) return;
        diaIniciado = true;

        if (diaActual > diaMaximo)
        {
            Debug.Log("¡Juego completado!");
            OnAllDaysCompleted?.Invoke();
            return;
        }

        Debug.Log($"--- Iniciando Día {diaActual} ---");
        
        // Ajustar dificultad según el día
        AjustarDificultad();
        MostrarConfiguracionDelDia();

        // Notificar a otros sistemas (como EconomyManager)
        if (EconomyManager.instance != null)
        {
            EconomyManager.instance.IniciarNuevoDia();
        }

        OnDayStarted?.Invoke(diaActual);
    }

    private void AjustarDificultad()

    {
        // En base al día actual, aumentamos la dificultad.
        // Día 1 es fácil (tutorial), el Día 5 es el más difícil.

        if (cameraThreatManager != null)
        {
            cameraThreatManager.oxygenWarningTime = 30f - (diaActual) * 5f;
            cameraThreatManager.oxygenGameOverTime = 60f - (diaActual) * 10f;

            cameraThreatManager.PhaseInterval = 25f - (diaActual - 1) * 4f;

            cameraThreatManager.FinalPhaseWaitTime = 8f - (diaActual - 1) * 1f;
        }
        
        float factorDificultad = (float)(diaActual - 1) / (diaMaximo - 1); // 0 a 1
        

        if (queueSystem != null)
        {
            // Más probabilidad de que generen estudiantes rápido
            queueSystem.probabilidadDuranteAtencion = Mathf.Lerp(0.3f, 0.8f, factorDificultad);
            
            // Insertar colados más a menudo en días difíciles
            // En el update del queueSystem no hay un método automático, pero podríamos añadirlo.
        }

        if (carnetGenerator != null)
        {
            // Aumentar la probabilidad de errores en el carnet
            carnetGenerator.probErrorIdentificacion = Mathf.Lerp(0.02f, 0.15f, factorDificultad);
            carnetGenerator.probErrorArea = Mathf.Lerp(0.05f, 0.20f, factorDificultad);
            carnetGenerator.probErrorTurno = Mathf.Lerp(0.05f, 0.25f, factorDificultad);
            carnetGenerator.probFechaVencida = Mathf.Lerp(0.05f, 0.20f, factorDificultad);
        }
        
        Debug.Log($"Dificultad ajustada para el día {diaActual} (Factor: {factorDificultad})");
    }

    public void EndDay()
    {
        Debug.Log($"--- Fin del Día {diaActual} ---");
        
        if (queueSystem != null)
        {
            queueSystem.TerminarAtencion();
        }
        
        if (RevisionManager.instance != null)
        {
            RevisionManager.instance.LimpiarMesa();
        }

        if (EconomyManager.instance != null)
        {
            EconomyManager.instance.AplicarPagoAlDineroTotal();
        }

        if (EndDayScene.instance != null)
        {
            EndDayScene.instance.MostrarResumen(diaActual);
        }
    }

    public void AvanzarSiguienteDia()
    {
        // Revisar Game Over por bancarrota
        /*if (EconomyManager.instance != null && EconomyManager.instance.dineroTotal < 0)
        {
            if (EndDayScene.instance != null) EndDayScene.instance.MostrarGameOver();
            return;
        }*/

        // Revisar si completó todos los días (Victoria)
        if (diaActual >= diaMaximo)
        {
            if (EndDayScene.instance != null && EndDayScene.instance.panelFinDeDia != null)
            {
                EndDayScene.instance.panelFinDeDia.SetActive(false);
            }

            if (EndingManager.instance != null)
            {
                EndingManager.instance.MostrarFinal();
            }
            else
            {
                Debug.LogWarning("No existe EndingManager en la escena.");
            }

            return;
        }

        // Si todo está bien, avanzar de día
        diaActual++;
        diaIniciado = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void MostrarConfiguracionDelDia()
    {
        string mensaje = $"=== CONFIGURACIÓN DEL DÍA {diaActual} ===\n";

        if (queueSystem != null)
        {
            mensaje +=
                $"Probabilidad cola antes de atención: {queueSystem.probabilidadAntesDeAtencion:P0}\n" +
                $"Probabilidad cola durante atención: {queueSystem.probabilidadDuranteAtencion:P0}\n" +
                $"Segundos entre intentos: {queueSystem.segundosEntreIntentos:0.00}\n";
        }
        else
        {
            mensaje += "QueueSystem: NO ENCONTRADO\n";
        }

        if (carnetGenerator != null)
        {
            mensaje +=
                $"Error identificación: {carnetGenerator.probErrorIdentificacion:P0}\n" +
                $"Error área: {carnetGenerator.probErrorArea:P0}\n" +
                $"Error turno: {carnetGenerator.probErrorTurno:P0}\n" +
                $"Carnet vencido: {carnetGenerator.probFechaVencida:P0}\n";
        }
        else
        {
            mensaje += "CarnetGenerator: NO ENCONTRADO\n";
        }

        if (cameraThreatManager != null)
        {
            mensaje +=
                $"Oxígeno para Game Over: {cameraThreatManager.oxygenGameOverTime:0.00} s\n" +
                $"Intervalo entre fases: {cameraThreatManager.PhaseInterval:0.00} s\n" +
                $"Espera en fase final: {cameraThreatManager.FinalPhaseWaitTime:0.00} s\n";
        }
        else
        {
            mensaje += "CameraThreatManager: NO ENCONTRADO\n";
        }

        Debug.Log(mensaje);
    }
}
