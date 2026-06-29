using UnityEngine;
using System.Collections.Generic;

public class ColaLibreManager : MonoBehaviour
{
    [Header("Referencias")]
    public QueueSystem queueSystem;
    public AlumnoGenerator alumnoGenerator;
    public CarnetGenerator carnetGenerator;

    [Header("Cola libre")]
    public List<PersonaEnCola> colaLibre = new List<PersonaEnCola>();

    public int maxPersonasEnColaLibre = 100;

    [Header("Cantidad total esperada")]
    public int minimoPersonasTotales = 40;
    public int maximoPersonasTotales = 60;

    private int totalPersonasQueLlegaran;
    private int personasCreadas = 0;

    [Header("Horario de generación")]
    public int horaInicioGeneracion = 11;
    public int minutoInicioGeneracion = 50;

    [Header("Generación por probabilidad")]
    public float segundosEntreIntentos = 2f;

    [Header("Probabilidad de llegada")]
    [Range(0f, 1f)]
    public float probabilidadInicial = 0.9f;

    [Range(0f, 1f)]
    public float probabilidadActual = 0.9f;

    [Range(0f, 1f)]
    public float factorReduccionPorTanda = 0.5f;

    private float timerGeneracion;

    [Header("Tandas hacia cola principal")]
    public int personasPorTanda = 15;
    public int minutosEntreTandas = 15;

    public int horaPrimeraTanda = 12;
    public int minutoMinPrimeraTanda = 2;
    public int minutoMaxPrimeraTanda = 10;

    private int proximaTandaEnMinutos;
    private int tiempoActualEnMinutos;

    private bool generacionActiva = false;
    private bool colaLibreTerminada = false;

    private int personasLiberadasTotal = 0;

    private void OnEnable()
    {
        GameClock.OnTimeChanged += RevisarHora;
    }

    private void OnDisable()
    {
        GameClock.OnTimeChanged -= RevisarHora;
    }

    private void Awake()
    {
        probabilidadActual = probabilidadInicial;

        int minutoPrimeraTanda = UnityEngine.Random.Range(
            minutoMinPrimeraTanda,
            minutoMaxPrimeraTanda + 1
        );

        proximaTandaEnMinutos = horaPrimeraTanda * 60 + minutoPrimeraTanda;

        totalPersonasQueLlegaran = UnityEngine.Random.Range(
            minimoPersonasTotales,
            maximoPersonasTotales + 1
        );

        Debug.Log("Total de personas que llegarán a cola libre: " + totalPersonasQueLlegaran);
        Debug.Log("Primera tanda programada a las 12:" + minutoPrimeraTanda.ToString("00"));
    }

    private void Update()
    {
        IntentarGenerarPersonaLibre();
    }

    private void RevisarHora(int hora, int minuto)
    {
        tiempoActualEnMinutos = hora * 60 + minuto;

        int inicioGeneracion = horaInicioGeneracion * 60 + minutoInicioGeneracion;

        if (!generacionActiva && tiempoActualEnMinutos >= inicioGeneracion)
        {
            generacionActiva = true;
            Debug.Log("Cola libre empezó a formarse.");
        }

        if (colaLibreTerminada) return;

        if (generacionActiva && tiempoActualEnMinutos >= proximaTandaEnMinutos)
        {
            LiberarTandaHaciaColaPrincipal();
            proximaTandaEnMinutos = tiempoActualEnMinutos + minutosEntreTandas;
        }
    }

    private void IntentarGenerarPersonaLibre()
    {
        if (!generacionActiva) return;
        if (colaLibreTerminada) return;

        if (personasCreadas >= totalPersonasQueLlegaran)
        {
            RevisarSiColaLibreTermino();
            return;
        }

        if (colaLibre.Count >= maxPersonasEnColaLibre) return;

        timerGeneracion += Time.deltaTime;

        if (timerGeneracion < segundosEntreIntentos) return;

        timerGeneracion = 0f;

        if (UnityEngine.Random.value < probabilidadActual)
        {
            PersonaEnCola nuevaPersona = CrearPersonaLibre();
            colaLibre.Add(nuevaPersona);

            personasCreadas++;

            Debug.Log(
                "Llegó persona a cola libre. Cola libre: " + colaLibre.Count +
                " | Creadas: " + personasCreadas + "/" + totalPersonasQueLlegaran +
                " | Prob: " + probabilidadActual
            );
        }
    }

    private void LiberarTandaHaciaColaPrincipal()
    {
        if (colaLibre.Count == 0)
        {
            Debug.Log("No hay nadie en cola libre para liberar esta tanda.");
            RevisarSiColaLibreTermino();
            return;
        }

        int cantidadALiberar = Mathf.Min(personasPorTanda, colaLibre.Count);

        int liberados = 0;

        for (int i = 0; i < cantidadALiberar; i++)
        {
            PersonaEnCola persona = colaLibre[0];
            colaLibre.RemoveAt(0);

            bool agregada = queueSystem.AgregarPersonaACola(persona);

            if (agregada)
            {
                liberados++;
                personasLiberadasTotal++;
            }
            else
            {
                colaLibre.Insert(0, persona);
                Debug.Log("La cola principal se llenó. Se detiene la liberación de tanda.");
                break;
            }
        }

        Debug.Log(
            "Tanda liberada hacia cola principal: " + liberados +
            " | Quedan en cola libre: " + colaLibre.Count +
            " | Personas liberadas total: " + personasLiberadasTotal
        );

        if (liberados > 0)
            {
                ReducirProbabilidadPorTanda();
            }
        
        RevisarSiColaLibreTermino();
    }

    private PersonaEnCola CrearPersonaLibre()
    {
        Alumno alumno = alumnoGenerator.GenerarAlumno();
        CarnetData carnet = carnetGenerator.GenerarCarnet(alumno);

        PersonaEnCola persona = new PersonaEnCola();

        persona.alumnoReal = alumno;
        persona.carnetMostrado = carnet;
        persona.esColado = false;
        persona.fueDetectadoPorCamara = false;

        return persona;
    }

    private void RevisarSiColaLibreTermino()
    {
        if (personasCreadas >= totalPersonasQueLlegaran && colaLibre.Count == 0)
        {
            colaLibreTerminada = true;
            Debug.Log("Cola libre terminada. Ya no se generan más personas.");
        }
    }

    private void ReducirProbabilidadPorTanda()
    {
        probabilidadActual *= factorReduccionPorTanda;

        Debug.Log("Nueva probabilidad de llegada a cola libre: " + probabilidadActual);
    }
}