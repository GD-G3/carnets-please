using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RevisionManager : MonoBehaviour
{
    public static RevisionManager instance;

    [Header("Persona a revisar")]
    public PersonaEnCola personaActual;

    [Header("Carnet visual")]
    public Carnet carnetVisual;
    private CarnetData carnetActual;

    [Header("Imagen del carnet")]
    public Image fotoCarnetImage;
    public FotoDatabase fotoDatabase;

    [Header("Resultados")]
    public TMP_Text textoResultados;

    [Header("Configuracion")]
    public int areaGlobal = 1;

    [Header("Fecha actual")]
    public int diaActual = 21;
    public int mesActual = 6;
    public int anioActual = 2026;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RecibirPersona(PersonaEnCola nuevaPersona)
    {
        personaActual = nuevaPersona;
        carnetActual = null;

        if (carnetVisual != null)
        {
            carnetVisual.datos = nuevaPersona.carnetMostrado;
        }
        else
        {
            Debug.LogWarning("No hay Carnet visual asignado en RevisionManager.");
        }

        Debug.Log("Nueva persona recibida para revision.");
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

        Debug.Log("Datos mostrados en la pantalla");
    }

   public void Permitir()
    {
        if (!PuedeDecidir()) return;

        bool debePasar = DebePasar();

        if (debePasar)
        {
            Debug.Log("Correcto: debia pasar.");
            textoResultados.text = "Correcto: debia pasar.";
        }
        else
        {
            Debug.Log("Error: no debia pasar.");
            textoResultados.text = "Error: no debia pasar.";
        }

        SiguientePersona();
    }

    public void Rechazar()
    {
        if (!PuedeDecidir()) return;

        bool debePasar = DebePasar();

        if (!debePasar)
        {
            Debug.Log("Correcto: debia ser rechazado.");
            textoResultados.text = "Correcto: debia ser rechazado.";
        }
        else
        {
            Debug.Log("Error: si debia pasar.");
            textoResultados.text = "Error: si debia pasar.";
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

        if (carnetActual.turno <= GameClock.TurnoGlobal) return false;

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

        if (QueueSystem.instance != null)
        {
            QueueSystem.instance.MandarSiguientePersona();
        }
        else
        {
            Debug.LogWarning("No existe QueueSystem en la escena.");
        }
    }


}