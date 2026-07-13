using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Este script gestiona las amenazas de los colados en las cámaras, la ventana y el sistema de oxígeno. También maneja el contador global de colados y el panel de Game Over.
public class CameraThreatManager : MonoBehaviour
{
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
    public float cam1PhaseInterval = 5f;
    public float cam1FinalPhaseWaitTime = 4f;

    [Header("Student Threat - Camera 2")]
    public Image studentCam2Image;
    public Button studentCam2Button;
    public Sprite[] studentCam2PhaseSprites;
    public float cam2PhaseInterval = 5f;
    public float cam2FinalPhaseWaitTime = 4f;

    [Header("Global Intruder Counter")]
    public TMP_Text intruderCounterText;
    public int maxIntrudersAllowed = 3;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private bool isWindowClosed = false;
    private float closedWindowTimer = 0f;

    private int intrudersInside = 0;

    private int cam1Phase = 0;
    private float cam1PhaseTimer = 0f;
    private bool cam1WaitingFinalAttempt = false;
    private float cam1FinalAttemptTimer = 0f;

    private int cam2Phase = 0;
    private float cam2PhaseTimer = 0f;
    private bool cam2WaitingFinalAttempt = false;
    private float cam2FinalAttemptTimer = 0f;

    private bool gameOver = false;

    private void Start()
    {
        if (windowToggleButton != null)
        {
            windowToggleButton.onClick.RemoveAllListeners();
            windowToggleButton.onClick.AddListener(ToggleWindow);
        }

        if (studentCam2Button != null)
        {
            studentCam2Button.onClick.RemoveAllListeners();
            studentCam2Button.onClick.AddListener(ExpelCam2Intruder);
        }

        SetWindowOpen();
        ResetCam1Threat();
        ResetCam2Threat();
        UpdateIntruderCounterText();
        UpdateCameraLayers();

        if (oxygenWarningText != null)
        {
            oxygenWarningText.gameObject.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameOver) return;

        UpdateCameraLayers();
        UpdateOxygenSystem();
        UpdateCam1Threat();
        UpdateCam2Threat();
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

        if (oxygenWarningText != null)
        {
            bool shouldShowWarning = viewingCamera1 && isWindowClosed && closedWindowTimer >= oxygenWarningTime;
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
        if (studentCam1PhaseSprites == null || studentCam1PhaseSprites.Length == 0) return;

        if (isWindowClosed && !cam1WaitingFinalAttempt)
        {
            return;
        }

        if (cam1WaitingFinalAttempt)
        {
            cam1FinalAttemptTimer += Time.deltaTime;

            if (cam1FinalAttemptTimer >= cam1FinalPhaseWaitTime)
            {
                ResolveCam1FinalAttempt();
            }

            return;
        }

        cam1PhaseTimer += Time.deltaTime;

        if (cam1PhaseTimer >= cam1PhaseInterval)
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
            Debug.Log("El alumno no pudo entrar por la ventana y se retiró.");
            ResetCam1Threat();
        }
        else
        {
            Debug.Log("Un alumno se coló por la ventana.");
            AddIntruderInside();
            ResetCam1Threat();
        }
    }

    private void ResetCam1Threat()
    {
        cam1Phase = 0;
        cam1PhaseTimer = 0f;
        cam1WaitingFinalAttempt = false;
        cam1FinalAttemptTimer = 0f;
        UpdateCam1Sprite();
    }

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
        if (studentCam2PhaseSprites == null || studentCam2PhaseSprites.Length == 0) return;

        if (cam2WaitingFinalAttempt)
        {
            cam2FinalAttemptTimer += Time.deltaTime;

            if (cam2FinalAttemptTimer >= cam2FinalPhaseWaitTime)
            {
                ResolveCam2FinalAttempt();
            }

            return;
        }

        cam2PhaseTimer += Time.deltaTime;

        if (cam2PhaseTimer >= cam2PhaseInterval)
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
        Debug.Log("Un colado se metió desde la cámara 2.");
        AddIntruderInside();
        ResetCam2Threat();
    }

    public void ExpelCam2Intruder()
    {
        if (!IsViewingCamera2()) return;

        Debug.Log("Colado de cámara 2 expulsado.");
        ResetCam2Threat();
    }

    private void ResetCam2Threat()
    {
        cam2Phase = 0;
        cam2PhaseTimer = 0f;
        cam2WaitingFinalAttempt = false;
        cam2FinalAttemptTimer = 0f;
        UpdateCam2Sprite();
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
        intrudersInside++;
        UpdateIntruderCounterText();

        if (intrudersInside >= maxIntrudersAllowed)
        {
            TriggerGameOver("Game Over: demasiados colados entraron.");
        }
    }

    private void UpdateIntruderCounterText()
    {
        if (intruderCounterText != null)
        {
            intruderCounterText.text = $"Colados: {intrudersInside} / {maxIntrudersAllowed}";
        }
    }

    private void TriggerGameOver(string reason)
    {
        gameOver = true;
        Debug.Log(reason);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}