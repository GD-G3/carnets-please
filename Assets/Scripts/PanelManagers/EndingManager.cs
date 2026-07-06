using UnityEngine;
using TMPro;

public enum TipoFinal
{
    Bueno,
    Neutral,
    Malo,
    Mediocre
}

public class EndingManager : MonoBehaviour
{
    public static EndingManager instance;

    [Header("Paneles")]
    public GameObject panelFinal;
    public TMP_Text textoFinal;

    [Header("Condiciones final bueno")]
    public float dineroMinimoFinalBueno = 500f;
    public int faltasMaximasFinalBueno = 5;
    public int personasPermitidasMinimas = 10;

    [Header("Condiciones final malo")]
    public int faltasMinimasFinalMalo = 30;
    public int coladosPermitidosMinimos = 10;
    public int rechazosInjustificadosMinimos = 20;
    public int advertenciasNegativasMinimas = 3;

    [Header("Condiciones final mediocre")]
    public int personasPermitidasMaximasMediocre = 3;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelFinal != null)
        {
            panelFinal.SetActive(false);
        }
    }

    public void MostrarFinal()
    {
        TipoFinal tipoFinal = CalcularFinal();

        if (panelFinal != null)
        {
            panelFinal.SetActive(true);
        }

        if (textoFinal != null)
        {
            textoFinal.text = ObtenerTextoFinal(tipoFinal);
        }

        Time.timeScale = 0f;
    }

    private TipoFinal CalcularFinal()
    {
        GameStatsManager stats = GameStatsManager.instance;
        EconomyManager economy = EconomyManager.instance;

        if (stats == null)
        {
            Debug.LogError("No existe GameStatsManager. Por eso se está devolviendo final malo.");
            return TipoFinal.Malo;
        }

        if (economy == null)
        {
            Debug.LogError("No existe EconomyManager. Por eso se está devolviendo final malo.");
            return TipoFinal.Malo;
        }

        int totalFaltas = stats.TotalFaltas();

        Debug.Log(
            "ESTADISTICAS FINALES\n" +
            "Personas permitidas: " + stats.personasPermitidas + "\n" +
            "Colados permitidos: " + stats.coladosPermitidos + "\n" +
            "Intrusos no detenidos: " + stats.intrusosNoDetenidos + "\n" +
            "Rechazos injustificados: " + stats.rechazosInjustificados + "\n" +
            "Total faltas: " + totalFaltas + "\n" +
            "Dias con dinero negativo: " + economy.diasConDineroNegativo + "\n" +
            "Dinero total: " + economy.dineroTotal
        );

        // 1. Final mediocre: casi no dejó pasar a nadie.
        if (stats.personasPermitidas <= personasPermitidasMaximasMediocre &&
            stats.coladosPermitidos == 0 && stats.intrusosNoDetenidos == 0)
        {
            return TipoFinal.Mediocre;
        }

        // 2. Final malo: demasiadas faltas acumuladas.
        else if (totalFaltas >= faltasMinimasFinalMalo ||
            stats.coladosPermitidos >= coladosPermitidosMinimos ||
            stats.rechazosInjustificados >= rechazosInjustificadosMinimos ||
            economy.diasConDineroNegativo >= advertenciasNegativasMinimas)
        {
            return TipoFinal.Malo;
        }

        // 3. Final bueno: buen dinero y buen desempeño.
        else if (economy.dineroTotal >= dineroMinimoFinalBueno &&
            totalFaltas <= faltasMaximasFinalBueno &&
            stats.personasPermitidas >= personasPermitidasMinimas)
        {
            return TipoFinal.Bueno;
        }

        return TipoFinal.Neutral;
    }

    private string ObtenerTextoFinal(TipoFinal tipoFinal)
    {
        switch (tipoFinal)
        {
            case TipoFinal.Bueno:
                return
                    "<size=42><b>FINAL BUENO</b></size>\n\n" +
                    "Terminaste los cinco días con un buen desempeño.\n" +
                    "El comedor mantuvo el orden, los alumnos autorizados pudieron entrar " +
                    "y las pérdidas fueron controladas.\n\n" +
                    "La administración decide renovarte el contrato.";

            case TipoFinal.Malo:
                return
                    "<size=42><b>FINAL MALO</b></size>\n\n" +
                    "Aunque llegaste al quinto día, tu gestión dejó demasiadas fallas.\n" +
                    "Hubo colados, rechazos injustificados o pérdidas acumuladas.\n\n" +
                    "La administración decide retirarte del puesto.";

            case TipoFinal.Mediocre:
                return
                    "<size=42><b>FINAL MEDIOCRE</b></size>\n\n" +
                    "No dejaste pasar colados, pero tampoco hiciste funcionar el comedor.\n" +
                    "Muy pocos alumnos pudieron entrar y el servicio fue prácticamente inútil.\n" +
                    "Las pérdidas fueron controladas?\n\n" + 
                    "Resultado: nadie se coló, pero nadie comió.";

            case TipoFinal.Neutral:
                return
                    "<size=42><b>FINAL NEUTRAL</b></size>\n\n" +
                    "No hubo un buen desempeño, pero tampoco pueden decir que fue terrible.\n" +
                    "El comedor sobrevivio a duras penas, y no todos comieron.\n" +
                    "No hubieron perdidas, pero tampoco ganancias.\n\n" +
                    "Dudas sobre si te reclutaras de nuevo";

            default:
                return "Final no encontrado.";
        }
    }
}