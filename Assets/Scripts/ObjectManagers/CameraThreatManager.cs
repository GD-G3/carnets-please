using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Este script gestiona las amenazas de los colados en las c�maras, la ventana y el sistema de ox�geno. Tambi�n maneja el contador global de colados y el panel de Game Over.
public class CameraThreatManager : MonoBehaviour
{
    public enum TipoEventoColado
    {
        Ninguno,
        Camara1,
        Camara2
    }

    [Header("Camera Detection")]
    public Image activeCameraImage;
    public Sprite camera1Sprite;
    public Sprite camera2Sprite;

    [Header("Threat Layers")]
    public GameObject camera1ThreatLayer;
    public GameObject camera2ThreatLayer;

    [Header("Window System - Camera 1")]
    public Image windowImage;
    public Sprite windowOpenSprite;
    public Sprite windowClosedSprite;
    public Button windowToggleButton;
    public TMP_Text windowToggleButtonText;

    [Header("Oxygen System")]
    public TMP_Text oxygenWarningText;
    public float oxygenWarningTime = 30f;
    public float oxygenGameOverTime = 60f;

    [Header("Student Threat - Camera 1")]
    public Image studentCam1Image;
    public Sprite[] studentCam1PhaseSprites;

    [Header("Student Threat - Camera 2")]
    public Image studentCam2Image;
    public Button studentCam2Button;
    public Sprite[] studentCam2PhaseSprites;

    public float PhaseInterval = 25f;
    public float FinalPhaseWaitTime = 8f;

    [Header("Generación de eventos")]
    public float segundosEntreIntentosEvento = 5f;


    [Tooltip("Índice 0 = día 1, índice 1 = día 2, etc.")]
    [Range(0f, 1f)]
    public float[] probabilidadEventoPorDia =
    {
        0.05f, // Día 1
        0.10f, // Día 2
        0.20f, // Día 3
        0.35f, // Día 4
        0.50f  // Día 5
    };

    [Range(0f, 1f)]
    public float probabilidadCamara1 = 0.5f;

    [Header("Estado del evento")]
    public TipoEventoColado eventoActual = TipoEventoColado.Ninguno;
    public bool eventoActivo = false;

    private bool isWindowClosed = false;
    private float closedWindowTimer = 0f;

    private bool eventosHabilitados = false;
    private float timerIntentoEvento = 0f;

    private int cam1Phase = 0;
    private float cam1PhaseTimer = 0f;
    private bool cam1WaitingFinalAttempt = false;
    private float cam1FinalAttemptTimer = 0f;

    private int cam2Phase = 0;
    private float cam2PhaseTimer = 0f;
    private bool cam2WaitingFinalAttempt = false;
    private float cam2FinalAttemptTimer = 0f;

    private bool gameOver = false;

    private void OnEnable()
    {
        GameClock.OnGameStarted += HabilitarEventos;
    }

    private void OnDisable()
    {
        GameClock.OnGameStarted -= HabilitarEventos;
    }

    private void Start()
    {
        if (windowToggleButton != null)
        {
            windowToggleButton.onClick.RemoveListener(ToggleWindow);
            windowToggleButton.onClick.AddListener(ToggleWindow);
        }

        if (studentCam2Button != null)
        {
            studentCam2Button.onClick.RemoveListener(ExpelCam2Intruder);
            studentCam2Button.onClick.AddListener(ExpelCam2Intruder);
        }

        SetWindowOpen();
        OcultarAmenazas();
        UpdateCameraLayers();

        if (oxygenWarningText != null)
        {
            oxygenWarningText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameOver) return;

        UpdateCameraLayers();
        UpdateOxygenSystem();

        if (!eventosHabilitados) return;
        if (!eventoActivo)
        {
            IntentarIniciarEvento();
            return;
        }

        switch (eventoActual)
        {
            case TipoEventoColado.Camara1:
                UpdateCam1Threat();
                break;

            case TipoEventoColado.Camara2:
                UpdateCam2Threat();
                break;
        }
    }

    private void HabilitarEventos()
    {
        eventosHabilitados = true;
        timerIntentoEvento = 0f;

        Debug.Log("Eventos de colados habilitados.");
    }

    private void IntentarIniciarEvento()
    {
        timerIntentoEvento += Time.deltaTime;

        if (timerIntentoEvento < segundosEntreIntentosEvento)
        {
            return;
        }

        timerIntentoEvento = 0f;

        float probabilidadActual = ObtenerProbabilidadDelDia();

        if (UnityEngine.Random.value >= probabilidadActual)
        {
            return;
        }
        

        if (UnityEngine.Random.value < probabilidadCamara1)
        {
            IniciarEventoCamara1();
        }
        else
        {
            IniciarEventoCamara2();
        }
    }

    private int ObtenerDiaActual()
    {
        if (DayManager.instance == null)
        {
            return 1;
        }

        return DayManager.instance.diaActual;
    }

    private float ObtenerProbabilidadDelDia()
    {
        if (probabilidadEventoPorDia == null || probabilidadEventoPorDia.Length == 0)
        {
            return 0f;
        }

        int indice = Mathf.Clamp(ObtenerDiaActual() - 1, 0, probabilidadEventoPorDia.Length - 1);
        return probabilidadEventoPorDia[indice];
    }

    private void IniciarEventoCamara1()
    {
        eventoActivo = true;
        eventoActual = TipoEventoColado.Camara1;

        cam1Phase = 0;
        cam1PhaseTimer = 0f;
        cam1WaitingFinalAttempt = false;
        cam1FinalAttemptTimer = 0f;

        if (studentCam1Image != null)
        {
            studentCam1Image.gameObject.SetActive(true);
        }

        UpdateCam1Sprite();

        Debug.Log("Comenzó un evento de colado en cámara 1.");
    }

    private void IniciarEventoCamara2()
    {
        eventoActivo = true;
        eventoActual = TipoEventoColado.Camara2;

        cam2Phase = 0;
        cam2PhaseTimer = 0f;
        cam2WaitingFinalAttempt = false;
        cam2FinalAttemptTimer = 0f;

        if (studentCam2Image != null)
        {
            studentCam2Image.gameObject.SetActive(true);
        }

        UpdateCam2Sprite();

        Debug.Log("Comenzó un evento de colado en cámara 2.");
    }

    private void FinalizarEvento(string motivo)
    {

        Debug.LogWarning(
        "Finalizando evento de colado. " +
        "Evento: " + eventoActual +
        " | Motivo: " + motivo
    );

        eventoActivo = false;
        eventoActual = TipoEventoColado.Ninguno;
        timerIntentoEvento = 0f;

        cam1Phase = 0;
        cam1PhaseTimer = 0f;
        cam1WaitingFinalAttempt = false;
        cam1FinalAttemptTimer = 0f;

        cam2Phase = 0;
        cam2PhaseTimer = 0f;
        cam2WaitingFinalAttempt = false;
        cam2FinalAttemptTimer = 0f;

        OcultarAmenazas();

        Debug.Log("Evento de colado finalizado.");
    }

    private void OcultarAmenazas()
    {
        if (studentCam1Image != null)
        {
            studentCam1Image.gameObject.SetActive(false);
        }

        if (studentCam2Image != null)
        {
            studentCam2Image.gameObject.SetActive(false);
        }
    }

    private void UpdateCameraLayers()
    {
        bool viewingCamera1 = IsViewingCamera1();
        bool viewingCamera2 = IsViewingCamera2();

        if (camera1ThreatLayer != null)
        {
            camera1ThreatLayer.SetActive(viewingCamera1);
        }

        if (camera2ThreatLayer != null)
        {
            camera2ThreatLayer.SetActive(viewingCamera2);
        }

        if (windowToggleButton != null)
        {
            windowToggleButton.gameObject.SetActive(viewingCamera1);
        }

        if (studentCam2Button != null)
        {
            bool mostrarBoton =
                viewingCamera2 &&
                eventoActivo &&
                eventoActual == TipoEventoColado.Camara2;

            studentCam2Button.gameObject.SetActive(mostrarBoton);
        }

        if (oxygenWarningText != null)
        {
            bool shouldShowWarning = isWindowClosed && closedWindowTimer >= oxygenWarningTime;
            oxygenWarningText.gameObject.SetActive(shouldShowWarning);
        }
    }

    private bool IsViewingCamera1()
    {
        if (activeCameraImage == null || camera1Sprite == null) return false;
        return activeCameraImage.sprite == camera1Sprite;
    }

    private bool IsViewingCamera2()
    {
        if (activeCameraImage == null || camera2Sprite == null) return false;
        return activeCameraImage.sprite == camera2Sprite;
    }

    public void ToggleWindow()
    {
        if (isWindowClosed)
        {
            SetWindowOpen();
        }
        else
        {
            SetWindowClosed();
        }
    }

    private void SetWindowOpen()
    {
        isWindowClosed = false;
        closedWindowTimer = 0f;

        if (windowImage != null && windowOpenSprite != null)
        {
            windowImage.sprite = windowOpenSprite;
        }

        if (windowToggleButtonText != null)
        {
            windowToggleButtonText.text = "Cerrar ventana";
        }

        if (oxygenWarningText != null)
        {
            oxygenWarningText.gameObject.SetActive(false);
        }

        Debug.Log("AbrirVentana");
    }

    private void SetWindowClosed()
    {
        isWindowClosed = true;
        closedWindowTimer = 0f;

        if (windowImage != null && windowClosedSprite != null)
        {
            windowImage.sprite = windowClosedSprite;
        }

        if (windowToggleButtonText != null)
        {
            windowToggleButtonText.text = "Abrir ventana";
        }

        Debug.Log("CerrarVentana");
    }

    private void UpdateOxygenSystem()
    {
        if (!isWindowClosed)
        {
            closedWindowTimer = 0f;
            return;
        }

        closedWindowTimer += Time.deltaTime;

        if (closedWindowTimer >= oxygenGameOverTime)
        {
            TriggerGameOver("Game Over: mantuviste la ventana cerrada demasiado tiempo.");
        }
    }

    private void UpdateCam1Threat()
    {
  
        if (!eventoActivo || eventoActual != TipoEventoColado.Camara1)
        {
            return;
        }

        if (studentCam1PhaseSprites == null ||
            studentCam1PhaseSprites.Length == 0)
        {
            FinalizarEvento("El arreglo de sprites de cámara 1 está vacío.");
            return;
        }

        if (cam1WaitingFinalAttempt)
        {
            cam1FinalAttemptTimer += Time.deltaTime;

            if (cam1FinalAttemptTimer >= FinalPhaseWaitTime)
            {
                ResolveCam1FinalAttempt();
            }

            return;
        }

        cam1PhaseTimer += Time.deltaTime;

        if (cam1PhaseTimer >= PhaseInterval)
        {
            cam1PhaseTimer = 0f;
            AdvanceCam1Phase();
        }
    }

    private void AdvanceCam1Phase()
    {
        cam1Phase++;

        if (cam1Phase >= studentCam1PhaseSprites.Length)
        {
            cam1Phase = studentCam1PhaseSprites.Length - 1;
            cam1WaitingFinalAttempt = true;
            cam1FinalAttemptTimer = 0f;
        }

        UpdateCam1Sprite();
    }

    private void ResolveCam1FinalAttempt()
    {
        if (isWindowClosed)
        {
            FinalizarEvento("La ventana estaba cerrada.");
            return;
        }
        else
        {
            AddIntruderInside();
            FinalizarEvento("El colado entró por cámara 1.");
        }
    }

    /*private void ResetCam1Threat()
    {
        cam1Phase = 0;
        cam1PhaseTimer = 0f;
        cam1WaitingFinalAttempt = false;
        cam1FinalAttemptTimer = 0f;
        UpdateCam1Sprite();
    }*/


    private void UpdateCam1Sprite()
    {
        if (studentCam1Image == null) return;
        if (studentCam1PhaseSprites == null || studentCam1PhaseSprites.Length == 0) return;

        int safePhase = Mathf.Clamp(cam1Phase, 0, studentCam1PhaseSprites.Length - 1);
        studentCam1Image.sprite = studentCam1PhaseSprites[safePhase];
        studentCam1Image.gameObject.SetActive(true);
    }

    private void UpdateCam2Threat()
    {
        if (!eventoActivo || eventoActual != TipoEventoColado.Camara2)
        {
            return;
        }

        if (studentCam2PhaseSprites == null || studentCam2PhaseSprites.Length == 0)
        {
            FinalizarEvento("El arreglo de sprites de cámara 2 está vacío.");
            return;
        }

        if (cam2WaitingFinalAttempt)
        {
            cam2FinalAttemptTimer += Time.deltaTime;

            if (cam2FinalAttemptTimer >= FinalPhaseWaitTime)
            {
                ResolveCam2FinalAttempt();
            }

            return;
        }

        cam2PhaseTimer += Time.deltaTime;

        if (cam2PhaseTimer >= PhaseInterval)
        {
            cam2PhaseTimer = 0f;
            AdvanceCam2Phase();
        }
    }

    private void AdvanceCam2Phase()
    {
        cam2Phase++;

        if (cam2Phase >= studentCam2PhaseSprites.Length)
        {
            cam2Phase = studentCam2PhaseSprites.Length - 1;
            cam2WaitingFinalAttempt = true;
            cam2FinalAttemptTimer = 0f;
        }

        UpdateCam2Sprite();
    }

    private void ResolveCam2FinalAttempt()
    {
        AddIntruderInside();
        FinalizarEvento("El colado entró desde cámara 2.");
    }

    public void ExpelCam2Intruder()
    {
        if (!IsViewingCamera2()) return;

        if (!eventoActivo || eventoActual != TipoEventoColado.Camara2)
        {
            Debug.Log("No hay ningún colado en cámara 2.");
            return;
        }

        if (!cam2WaitingFinalAttempt)
        {
            Debug.Log("Todavía no puedes expulsarlo. No está en la fase final.");
            return;
        }

        FinalizarEvento("El jugador expulsó al intruso de cámara 2.");
    }


    private void UpdateCam2Sprite()
    {
        if (studentCam2Image == null) return;
        if (studentCam2PhaseSprites == null || studentCam2PhaseSprites.Length == 0) return;

        int safePhase = Mathf.Clamp(cam2Phase, 0, studentCam2PhaseSprites.Length - 1);
        studentCam2Image.sprite = studentCam2PhaseSprites[safePhase];
        studentCam2Image.gameObject.SetActive(true);
    }

    private void AddIntruderInside()
    {
        if (QueueSystem.instance == null) return;

        QueueSystem.instance.InsertarColado();

        Debug.Log(
            "El colado fue insertado en la cola principal."
        );
    }


    private void TriggerGameOver(string reason)
    {
        if (gameOver) return;

        gameOver = true;
        eventoActivo = false;
        eventosHabilitados = false;

        CancelInvoke();

        StopAllCoroutines();

        FinalizarEvento("Se produjo Game Over.");

        Debug.Log(reason);

        if (EndingManager.instance != null)
        {
            EndingManager.instance.MostrarFinalAhogamiento();
        }
        else
        {
            Debug.LogWarning(
                "No existe EndingManager. No se pudo mostrar el final por falta de oxígeno."
            );
        }
    }
}