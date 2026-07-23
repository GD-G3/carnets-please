using UnityEngine;
using System.Collections.Generic;

public class QueueSystem : MonoBehaviour
{
    public static QueueSystem instance;

    [Header("Cola")]
    public List<PersonaEnCola> cola = new List<PersonaEnCola>();
    public int maxPersonasEnCola = 80;

    [Header("Probabilidades por etapa")]
    [Range(0f, 1f)]
    public float probabilidadAntesDeAtencion = 0.9f;

    [Range(0f, 1f)]
    public float probabilidadDuranteAtencion = 0.5f;

    [Range(0f, 1f)]
    public float probabilidadGenerarEstudiante = 0.9f;
    
    public float segundosEntreIntentos = 3f;
    private float timerGeneracion;

    [Header("Generadores")]
    public AlumnoGenerator alumnoGenerator;
    public CarnetGenerator carnetGenerator;

    [Header("Revision")]
    public RevisionManager revisionManager;

    [Header("Personaje visual")]
    public QueueCharacterManager queueCharacterManager;

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
        cola.Clear();

        probabilidadGenerarEstudiante = probabilidadAntesDeAtencion;

        Debug.Log("QueueSystem iniciado. Probabilidad antes de atencion: " + probabilidadGenerarEstudiante);
    }

    private void Update()
    {
        IntentarGenerarEstudianteConTiempo();
    }

    private void IntentarGenerarEstudianteConTiempo()
    {
        if (cola.Count >= maxPersonasEnCola) return;

        timerGeneracion += Time.deltaTime;

        if (timerGeneracion >= segundosEntreIntentos)
        {
            timerGeneracion = 0f;

            if (UnityEngine.Random.value < probabilidadGenerarEstudiante)
            {
                PersonaEnCola nuevaPersona = CrearPersonaNormal();
                cola.Add(nuevaPersona);

                if (atencionActiva && !HayPersonaEnRevision())
                {
                    MandarSiguientePersona();
                }
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
        persona.vieneDeColaLibre = false;

        return persona;
    }

    public void EmpezarAtencion()
    {
        atencionActiva = true;

        probabilidadGenerarEstudiante = probabilidadDuranteAtencion;

        Debug.Log("🔥 EmpezarAtencion ejecutado");
        Debug.Log("Probabilidad durante atencion: " + probabilidadGenerarEstudiante);

        MandarSiguientePersona();
    }

    public void TerminarAtencion()
    {
        atencionActiva = false;
        cola.Clear();
        Debug.Log("Fin de la atencion. Cola vaciada.");
    }

    public void MandarSiguientePersona()
    {
        if (!atencionActiva)
        {
            Debug.Log("La atencion todavia no ha empezado.");
            return;
        }

        if (HayPersonaEnRevision())
        {
            Debug.Log("Ya hay una persona en revision.");
            return;
        }

        if (cola.Count == 0)
        {
            Debug.Log("No hay mas personas en cola.");
            return;
        }


        PersonaEnCola siguientePersona = cola[0];
        cola.RemoveAt(0);

        if (queueCharacterManager != null)
        {
            queueCharacterManager.PresentarPersona(siguientePersona);
        }
        else
        {
            Debug.LogWarning("QueueSystem no tiene RevisionManager asignado.");
        }

        Debug.Log("Persona enviada a revision.");
    }

    public bool AgregarPersonaACola(PersonaEnCola persona)
    {
        if (persona == null)
        {
            Debug.LogWarning("Intentaste agregar una persona nula a la cola.");
            return false;
        }

        if (cola.Count >= maxPersonasEnCola)
        {
            Debug.Log("La cola principal está llena.");
            return false;
        }

        cola.Add(persona);

        if (atencionActiva && !HayPersonaEnRevision())
        {
            MandarSiguientePersona();
        }

        return true;
    }

    public void InsertarColado()
    {
        PersonaEnCola colado = CrearPersonaNormal();
        colado.esColado = true;
        colado.vieneDeColaLibre = false;

        int posicion = UnityEngine.Random.Range(0, cola.Count + 1);
        cola.Insert(posicion, colado);

        Debug.Log("Colado insertado en la cola en la posicion: " + posicion);

        if (atencionActiva && !HayPersonaEnRevision())
        {
            MandarSiguientePersona();
        }
    }

    public void CambiarProbabilidadGeneracion(float nuevaProbabilidad)
    {
        probabilidadGenerarEstudiante = Mathf.Clamp01(nuevaProbabilidad);

        Debug.Log("Nueva probabilidad de generar estudiante: " + probabilidadGenerarEstudiante);
    }

    private bool HayPersonaEnRevision()
    {
        bool revisando = revisionManager != null && revisionManager.hayPersonaEnRevision;

        bool entrando = queueCharacterManager != null && queueCharacterManager.HayPersonaEntrando;

        return revisando || entrando;
    }
}
