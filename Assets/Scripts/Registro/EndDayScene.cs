using UnityEngine;
using TMPro;

public class EndDayScene : MonoBehaviour
{
    public static EndDayScene instance;

    public GameObject panelFinDeDia;
    public TextMeshProUGUI textoCompleto;

    [Header("Pantallas Finales")]
    public GameObject panelGameOver;
    public GameObject panelVictoria;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelFinDeDia != null) panelFinDeDia.SetActive(false);
        if (panelGameOver != null) panelGameOver.SetActive(false);
        if (panelVictoria != null) panelVictoria.SetActive(false);
    }

    public void MostrarResumen(int diaActual)
    {
        if (panelFinDeDia == null) return;
        
        panelFinDeDia.SetActive(true);

        if (EconomyManager.instance == null)
        {
            Debug.LogWarning("No se encuentra EconomyManager.");
            return;
        }

        float pagoBase = EconomyManager.instance.salarioBasePorDia;
        float pagoFinal = EconomyManager.instance.CalcularPagoDelDia();
        float dineroTotal = EconomyManager.instance.dineroTotal;

        string contenido = $"<size=40><b>Fin del Día {diaActual}</b></size>\n\n";
        contenido += $"Sueldo base: S/ {pagoBase}\n\n";

        var multas = EconomyManager.instance.ObtenerPenalizacionesDelDia();
        if (multas.Count == 0)
        {
            contenido += "<i>¡Excelente trabajo! No hubo errores hoy.</i>\n\n";
        }
        else
        {
            contenido += "<b>Descuentos por errores:</b>\n";
            foreach (var multa in multas)
            {
                contenido += $"- {multa.motivo}: -S/ {multa.monto}\n";
            }
            contenido += "\n";
        }

        contenido += $"<b>Ganancia de hoy: S/ {pagoFinal}</b>\n";
        contenido += $"<b>Dinero total ahorrado: S/ {dineroTotal}</b>";

        if (textoCompleto != null)
        {
            textoCompleto.text = contenido;
        }
    }

    public void MostrarGameOver()
    {
        if (panelGameOver != null) panelGameOver.SetActive(true);
    }

    public void MostrarVictoria()
    {
        if (panelVictoria != null) panelVictoria.SetActive(true);
    }

    public void ReiniciarJuego()
    {
        // Destruir todo
        if (DayManager.instance != null) Destroy(DayManager.instance.gameObject);
        if (EconomyManager.instance != null) Destroy(EconomyManager.instance.gameObject);

        // Volver al Menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void BotonSiguienteDia()
    {
        if (panelFinDeDia != null) panelFinDeDia.SetActive(false);

        if (DayManager.instance != null)
        {
            DayManager.instance.AvanzarSiguienteDia();
        }
    }
}
