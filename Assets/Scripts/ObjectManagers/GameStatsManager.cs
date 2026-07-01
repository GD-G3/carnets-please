using UnityEngine;

public class GameStatsManager : MonoBehaviour
{
    public static GameStatsManager instance;

    [Header("Estadisticas globales")]
    public int personasPermitidas = 0;
    public int personasRechazadas = 0;

    public int aprobacionesCorrectas = 0;
    public int rechazosCorrectos = 0;

    public int aprobacionesIncorrectas = 0;
    public int rechazosInjustificados = 0;

    public int coladosPermitidos = 0;
    public int intrusosNoDetenidos = 0;

    [Header("Dias")]
    public int diasPerfectos = 0;

    private int erroresDelDia = 0;

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

    public void RegistrarPermitir(bool fueCorrecto, bool eraColado)
    {
        personasPermitidas++;

        if (fueCorrecto)
        {
            aprobacionesCorrectas++;
        }
        else
        {
            aprobacionesIncorrectas++;
            erroresDelDia++;

            if (eraColado)
            {
                coladosPermitidos++;
            }
        }
    }

    public void RegistrarRechazar(bool fueCorrecto)
    {
        personasRechazadas++;

        if (fueCorrecto)
        {
            rechazosCorrectos++;
        }
        else
        {
            rechazosInjustificados++;
            erroresDelDia++;
        }
    }

    public void RegistrarIntrusoNoDetenido()
    {
        intrusosNoDetenidos++;
        erroresDelDia++;
    }

    public void CerrarDia()
    {
        if (erroresDelDia == 0)
        {
            diasPerfectos++;
        }

        erroresDelDia = 0;
    }

    public int TotalFaltas()
    {
        return aprobacionesIncorrectas + rechazosInjustificados + coladosPermitidos + intrusosNoDetenidos;
    }
}