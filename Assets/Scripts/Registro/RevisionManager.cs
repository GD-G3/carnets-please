using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RevisionManager : MonoBehaviour
{
    public static RevisionManager instance;

    [Header("Persona a revisar")]
    public PersonaEnCola personaActual;

    [Header("Carnet visual")]
    public Carnet carnetVisual;
    private CarnetData carnetActual;

    [Header("Posicion del carnet")]
    public Transform puntoCarnet;   

    [Header("Imagen del carnet")]
    public Image fotoCarnetImage;
    public FotoDatabase fotoDatabase;

    [Header("Resultados")]
    public TMP_Text textoResultados;

    [Header("Estado de revision")]
    public bool hayPersonaEnRevision = false;

    [Header("Configuracion")]
    public int areaGlobal = 1;

    [Header("Segundos de espera")]
    public float segundosEntreEstudiantes = 3f;

    [Header("Fecha actual")]
    public int diaActual = 21;
    public int mesActual = 6;
    public int anioActual = 2026;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        personaActual = null;
        carnetActual = null;
        hayPersonaEnRevision = false;

        if (carnetVisual != null)
        {
            carnetVisual.gameObject.SetActive(false);
        }

        if (fotoCarnetImage != null)
        {
            fotoCarnetImage.sprite = null;
            fotoCarnetImage.enabled = false;
        }

        if (textoResultados != null)
        {
            textoResultados.text = "Esperando inicio del turno...";
        }
    }

    public void RecibirPersona(PersonaEnCola nuevaPersona)
    {
        personaActual = nuevaPersona;
        carnetActual = null;
        hayPersonaEnRevision = true;

        if (carnetVisual != null)
        {
            carnetVisual.gameObject.SetActive(false);
        }

        if (fotoCarnetImage != null)
        {
            fotoCarnetImage.sprite = null;
            fotoCarnetImage.enabled = false;
        }

        StartCoroutine(MostrarCarnetDespuesDeEspera(nuevaPersona));

    }

    public void RecibirDatosEscaneados(CarnetData datos)
    {
         if (datos == null)
        {
            Debug.LogWarning("No se recibieron datos del carnet.");
            return;
        }

        carnetActual = datos;
        textoResultados.text = $"{datos.NombreTexto()}\t{datos.codigo}\nTurno {datos.turno}\nFacultad: {datos.facultad}\nCarrera: {datos.carrera}\nVence: {datos.diaVencimiento:00}/{datos.mesVencimiento:00}/{datos.anioVencimiento}";
        
        MostrarFotoDelCarnet(datos.fotoID);
    }

    private IEnumerator MostrarCarnetDespuesDeEspera(PersonaEnCola persona)
    {
        yield return new WaitForSeconds(segundosEntreEstudiantes);

        if (personaActual != persona)
        {
            yield break;
        }

        if (carnetVisual != null)
        {
            carnetVisual.datos = persona.carnetMostrado;
            carnetVisual.ResetearCarnet();

            if (puntoCarnet != null)
            {
                carnetVisual.transform.position = puntoCarnet.position;
                carnetVisual.transform.rotation = puntoCarnet.rotation;
                carnetVisual.transform.localScale = puntoCarnet.localScale;
            }

            carnetVisual.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No hay Carnet visual asignado en RevisionManager.");
        }

        if (textoResultados != null)
        {
            textoResultados.text = "Esperando escaneo del carnet...";
        }
    }

   public void Permitir()
    {
        if (!PuedeDecidir()) return;

        bool debePasar = DebePasar();

        if (debePasar)
        {
            textoResultados.text = "Correcto: debia pasar.";
        }
        else
        {
            textoResultados.text = "Error: no debia pasar.";
            
            if (EconomyManager.instance != null)
            {
                if (personaActual.esColado)
                {
                    EconomyManager.instance.RegistrarPenalizacion("Dejó entrar colado", EconomyManager.instance.multaPermitirColado);
                }
                else
                {
                    EconomyManager.instance.RegistrarPenalizacion("Aprobación incorrecta", EconomyManager.instance.multaAprobacionIncorrecta);
                }
            }
        }

        SiguientePersona();
    }

    public void Rechazar()
    {
        if (!PuedeDecidir()) return;

        bool debePasar = DebePasar();

        if (!debePasar)
        {
            textoResultados.text = "Correcto: debia ser rechazado.";
        }
        else
        {
            textoResultados.text = "Error: si debia pasar.";

            if (EconomyManager.instance != null)
            {
                EconomyManager.instance.RegistrarPenalizacion("Rechazo injustificado", EconomyManager.instance.multaRechazoInjustificado);
            }
        }

        SiguientePersona();
    }

    private bool PuedeDecidir()
    {
        if (personaActual == null)
        {
            textoResultados.text = "No hay persona actual.";
            return false;
        }

        if (carnetActual == null)
        {
            textoResultados.text = "Primero debes escanear el carnet.";
            return false;
        }

        return true;
    }

    private bool DebePasar()
    {
        Alumno alumnoReal = personaActual.alumnoReal;

        if (personaActual.esColado) return false;

        if (alumnoReal.fotoID != carnetActual.fotoID) return false;

        if (carnetActual.turno > GameClock.TurnoGlobal) return false;

        if (carnetActual.area != areaGlobal) return false;

        if (!CarnetVigente(carnetActual)) return false;

        return true;
    }

    private bool CarnetVigente(CarnetData carnet)
    {
        int fechaActual = anioActual * 10000 + mesActual * 100 + diaActual;

        int fechaVencimiento = carnet.anioVencimiento * 10000
                             + carnet.mesVencimiento * 100
                             + carnet.diaVencimiento;

        return fechaVencimiento >= fechaActual;
    }

    private void MostrarFotoDelCarnet(int fotoID)
    {
        if (fotoDatabase == null)
        {
            Debug.LogWarning("No hay FotoDatabase asignado en RevisionManager.");
            return;
        }

        if (fotoCarnetImage == null)
        {
            Debug.LogWarning("No hay Image asignada para mostrar la foto.");
            return;
        }

        FotoData foto = fotoDatabase.ObtenerFotoPorID(fotoID);

        if (foto == null)
        {
            Debug.LogWarning("No se encontro foto con ID: " + fotoID);
            fotoCarnetImage.sprite = null;
            return;
        }

        fotoCarnetImage.sprite = foto.sprite;
        fotoCarnetImage.enabled = true;
    }

    private void SiguientePersona()
    {
        Debug.Log("Siguiente persona...");
        personaActual = null;
        carnetActual = null;
        hayPersonaEnRevision = false;

        if (carnetVisual != null)
        {
            carnetVisual.ResetearCarnet();
            carnetVisual.gameObject.SetActive(false);
        }
        
        if (QueueSystem.instance != null)
        {
            QueueSystem.instance.MandarSiguientePersona();
        }
        else
        {
            Debug.LogWarning("No existe QueueSystem en la escena.");
        }
    }

    public void LimpiarMesa()
    {
        personaActual = null;
        carnetActual = null;

        if (carnetVisual != null)
        {
            carnetVisual.gameObject.SetActive(false);
        }

        if (fotoCarnetImage != null)
        {
            fotoCarnetImage.sprite = null;
            fotoCarnetImage.enabled = false;
        }

        if (textoResultados != null)
        {
            textoResultados.text = "Turno finalizado.";
        }
        
        StopAllCoroutines();
    }
}