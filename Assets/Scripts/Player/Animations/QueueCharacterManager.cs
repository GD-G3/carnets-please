using UnityEngine;

public class QueueCharacterManager : MonoBehaviour
{
    public static QueueCharacterManager instance;

    [Header("Personajes reutilizables")]
    public QueueCharacterUI personajeA;
    public QueueCharacterUI personajeB;

    [Header("Posiciones")]
    public RectTransform puntoEntrada;
    public RectTransform puntoRevision;
    public RectTransform puntoSalida;

    [Header("Revisión")]
    public RevisionManager revisionManager;

    private QueueCharacterUI personajeActual;
    private bool entradaEnCurso;

    public bool HayPersonaEntrando =>
        entradaEnCurso;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (personajeA != null)
        {
            personajeA.gameObject.SetActive(false);
        }

        if (personajeB != null)
        {
            personajeB.gameObject.SetActive(false);
        }
    }

    public void PresentarPersona(PersonaEnCola persona)
    {
        if (persona == null)
        {
            Debug.LogWarning("Se intentó presentar una persona nula.");
            return;
        }

        if (entradaEnCurso)
        {
            Debug.LogWarning("Ya existe una persona entrando.");
            return;
        }

        QueueCharacterUI personajeNuevo = ObtenerPersonajeDisponible();

        if (personajeNuevo == null)
        {
            Debug.LogWarning("Los dos personajes visuales están ocupados.");
            return;
        }

        QueueCharacterMovementUI movimiento = personajeNuevo.GetComponent<QueueCharacterMovementUI>();

        if (movimiento == null)
        {
            Debug.LogError(
                "El personaje no tiene " +
                "QueueCharacterMovementUI."
            );
            return;
        }

        entradaEnCurso = true;

        personajeNuevo.ConfigurarDesdePersona(persona);
        personajeNuevo.gameObject.SetActive(true);

        movimiento.ColocarEn(puntoEntrada);

        movimiento.MoverHacia(
            puntoRevision, false, 
            () =>
            {
                personajeActual = personajeNuevo;
                entradaEnCurso = false;

                if (revisionManager != null)
                {
                    revisionManager.RecibirPersona(persona);
                }
            }
        );
    }

    public void RetirarPersonaActual(bool fueAceptado)
    {
        if (personajeActual == null)
        {
            return;
        }

        QueueCharacterUI personajeQueSale = personajeActual;

        personajeActual = null;

        QueueCharacterMovementUI movimiento = personajeQueSale.GetComponent<QueueCharacterMovementUI>();

        if (movimiento == null)
        {
            personajeQueSale.gameObject.SetActive(false);
            return;
        }

        RectTransform destino = fueAceptado? puntoSalida: puntoEntrada;

        movimiento.MoverHacia(destino, !fueAceptado, 
        () =>
        {
            personajeQueSale.gameObject.SetActive(false);
        });
    }

    private QueueCharacterUI ObtenerPersonajeDisponible()
    {
        if (personajeA != null &&
            !personajeA.gameObject.activeSelf)
        {
            return personajeA;
        }

        if (personajeB != null &&
            !personajeB.gameObject.activeSelf)
        {
            return personajeB;
        }

        return null;
    }

}