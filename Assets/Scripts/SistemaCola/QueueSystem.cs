using UnityEngine;
using System.Collections.Generic;

public class QueueSystem : MonoBehaviour
{
    public static QueueSystem instance;

    [Header("Cola")]
    public List<PersonaEnCola> cola = new List<PersonaEnCola>();
    public int maxPersonasEnCola = 6;

    [Header("Generacion continua")]
    [Range(0f, 1f)]
    public float probabilidadGenerarEstudiante = 0.5f;
    public float segundosEntreIntentos = 3f;
    private float timerGeneracion;

    [Header("Generadores")]
    public AlumnoGenerator alumnoGenerator;
    public CarnetGenerator carnetGenerator;

    [Header("Revision")]
    public RevisionManager revisionManager;

    [Header("Configuracion")]
    public bool atencionActiva = false; //se activa a las 12

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        GameClock.OnGameStarted += EmpezarAtencion;
    }

    private void OnDisable()
    {
        GameClock.OnGameStarted -= EmpezarAtencion;
    }

    private void Start()
    {
        GenerarColaInicial();
    }

    private void Update()
    {
        IntentarGenerarEstudianteConTiempo();
    }

    private void GenerarColaInicial()
    {
        cola.Clear();

        for (int i = 0; i < maxPersonasEnCola; i++)
        {
            PersonaEnCola persona = CrearPersonaNormal();
            cola.Add(persona);
        }

        Debug.Log("Cola inicial generada. Personas en cola: " + cola.Count);
    }

    private void IntentarGenerarEstudianteConTiempo()
    {
        if (!atencionActiva) return;
        
        if (cola.Count >= maxPersonasEnCola) return;

        timerGeneracion += Time.deltaTime;

        if (timerGeneracion >= segundosEntreIntentos)
        {
            timerGeneracion = 0f;

            if (UnityEngine.Random.value < probabilidadGenerarEstudiante)
            {
                PersonaEnCola nuevaPersona = CrearPersonaNormal();
                cola.Add(nuevaPersona);

                Debug.Log("Nuevo estudiante generado. Personas en cola: " + cola.Count);

                if (!HayPersonaEnRevision())
                {
                    MandarSiguientePersona();
                }
            }
            else
            {
                Debug.Log("No se genero estudiante en este intento.");
            }
        }
    }

    private PersonaEnCola CrearPersonaNormal()
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

    public void EmpezarAtencion()
    {
        atencionActiva = true;
        Debug.Log("Empieza la atencion de la cola.");

        MandarSiguientePersona();
    }

    public void MandarSiguientePersona()
    {
        if (!atencionActiva)
        {
            Debug.Log("La atencion todavia no ha empezado.");
            return;
        }

        if (cola.Count == 0)
        {
            Debug.Log("No hay mas personas en cola.");
            return;
        }


        PersonaEnCola siguientePersona = cola[0];
        cola.RemoveAt(0);

        if (revisionManager != null)
        {
            revisionManager.RecibirPersona(siguientePersona);
        }
        else
        {
            Debug.LogWarning("QueueSystem no tiene RevisionManager asignado.");
        }

        Debug.Log("Persona enviada a revision.");
    }

    public void InsertarColado()
    {
        PersonaEnCola colado = CrearPersonaNormal();
        colado.esColado = true;

        int posicion = UnityEngine.Random.Range(0, cola.Count + 1);
        cola.Insert(posicion, colado);

        Debug.Log("Colado insertado en la cola en la posicion: " + posicion);
    }

    public void CambiarProbabilidadGeneracion(float nuevaProbabilidad)
    {
        probabilidadGenerarEstudiante = Mathf.Clamp01(nuevaProbabilidad);

        Debug.Log("Nueva probabilidad de generar estudiante: " + probabilidadGenerarEstudiante);
    }

    private bool HayPersonaEnRevision()
    {
        if (revisionManager == null) return false;

        return revisionManager.personaActual != null;
    }
}
