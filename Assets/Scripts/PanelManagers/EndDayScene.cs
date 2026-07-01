using UnityEngine;
using TMPro;

public class EndDayScene : MonoBehaviour
{
    public static EndDayScene instance;

    public GameObject panelFinDeDia;
    public TextMeshProUGUI textoCompleto;
    public TextMeshProUGUI textoResumenGanancias;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelFinDeDia != null) panelFinDeDia.SetActive(false);
    }

    public void MostrarResumen(int diaActual)
    {
        if (panelFinDeDia == null) return;
        
        panelFinDeDia.SetActive(true);
        Time.timeScale = 0f;

        if (EconomyManager.instance == null)
        {
            Debug.LogWarning("No se encuentra EconomyManager.");
            return;
        }

        float pagoBase = EconomyManager.instance.salarioBasePorDia;
        float pagoFinal = EconomyManager.instance.CalcularPagoDelDia();
        float dineroTotal = EconomyManager.instance.dineroTotal;

        string contenido = $"<size=40><b>Fin del Día {diaActual}</b></size>\n\n";
        string resumen = "";

        var multas = EconomyManager.instance.ObtenerPenalizacionesDelDia();

        if (multas.Count == 0)
        {
            contenido += "<i>¡Excelente trabajo! No hubo errores hoy.</i>\n\n";
        }
        else
        {
            contenido += "<b>Descuentos por errores:</b>\n\n";

            foreach (var multa in multas)
            {
                contenido += $"- {multa.motivo}: -S/ {multa.monto:0.00}\n";
            }

            contenido += "\n";
        }

        resumen += $"<b>Sueldo base:</b> S/ {pagoBase:0.00}\n";
        resumen += $"<b>Ganancia de hoy:</b> S/ {pagoFinal:0.00}\n";
        resumen += $"<b>Dinero total ahorrado:</b> S/ {dineroTotal:0.00}";

        if (EconomyManager.instance.dineroTotal < 0)
        {
            resumen += $"\n\n<color=red><b>Advertencia:</b> tiene {EconomyManager.instance.diasConDineroNegativo} multa(s) por saldo negativo.</color>";
        }

        if (textoCompleto != null)
        {
            textoCompleto.text = contenido;
        }

        if (textoResumenGanancias != null)
        {
            textoResumenGanancias.text = resumen;
        }
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
        Time.timeScale = 1f;

        if (panelFinDeDia != null) panelFinDeDia.SetActive(false);

        if (DayManager.instance != null)
        {
            DayManager.instance.AvanzarSiguienteDia();
        }
    }
}
