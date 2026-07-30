using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

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

    [Header("Resultados")]
    public TMP_Text textoResultados;
    public TMP_Text textoCola;

    [Header("Estado de revision")]
    public bool hayPersonaEnRevision = false;

    [Header("Configuracion")]
    public int areaGlobal = 1;

    [Header("Segundos de espera")]
    public float segundosEntreEstudiantes = 0f;

    [Header("Fecha actual")]
    public int diaActual = 21;
    public int mesActual = 6;
    public int anioActual = 2026;
    
    [Header("Personaje visual")]
    public QueueCharacterManager queueCharacterManager;

    [Header("Foto modular del carnet")]
    public GameObject fotoCarnetContainer;

    [Header("Foto modular mostrada en pantalla")]
    public Image cabezaFotoImage;
    public Image ojosFotoImage;
    public Image cejasFotoImage;
    public Image narizFotoImage;
    public Image bocaFotoImage;
    public Image cabelloFotoImage;

    [Header("Sprites fijos hombre")]
    public Sprite cabezaHombre;
    public Sprite narizHombre;

    [Header("Sprites fijos mujer")]
    public Sprite cabezaMujer;
    public Sprite narizMujer;

    [Header("Rasgos hombre")]
    public Sprite[] ojosHombre;
    public Sprite[] cejasHombre;
    public Sprite[] bocasHombre;
    public Sprite[] cabellosHombre;

    [Header("Rasgos mujer")]
    public Sprite[] ojosMujer;
    public Sprite[] cejasMujer;
    public Sprite[] bocasMujer;
    public Sprite[] cabellosMujer;

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

        if (fotoCarnetContainer != null)
        {
            fotoCarnetContainer.SetActive(false);
        }

    }

    public void RecibirPersona(PersonaEnCola nuevaPersona)
    {
        personaActual = nuevaPersona;
        carnetActual = null;
        hayPersonaEnRevision = true;
        textoCola.text = "";

        if (carnetVisual != null)
        {
            carnetVisual.gameObject.SetActive(false);
        }

        MostrarCarnet(nuevaPersona);

    }

    public void RecibirDatosEscaneados(CarnetData datos)
    {
         if (datos == null)
        {
            Debug.LogWarning("No se recibieron datos del carnet.");
            return;
        }

        carnetActual = datos;
        textoResultados.text = $"{datos.NombreTexto()}\t{datos.codigo}\nTurno {datos.turno}\nFacultad: {datos.facultad}\nArea: {datos.area}\nCarrera: {datos.carrera}\nVence: {datos.diaVencimiento:00}/{datos.mesVencimiento:00}/{datos.anioVencimiento}";
        
        if (personaActual != null && personaActual.vieneDeColaLibre) {
            textoCola.text = "COLA LIBRE";
        } else {
            textoCola.text = "";
        }

        if (fotoCarnetContainer != null)
        {
            fotoCarnetContainer.SetActive(true);
        }
        MostrarFotoCarnet(datos);
    }

    private void MostrarCarnet(PersonaEnCola persona)
    {
        if (personaActual != persona)
        {
            return;
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
    }

   public void Permitir()
    {
        if (!PuedeDecidir()) return;

        bool debePasar = DebePasar();

        if (GameStatsManager.instance != null)
        {
            GameStatsManager.instance.RegistrarPermitir(debePasar, personaActual.esColado);
        }

        if (!debePasar)
        {   
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

        SiguientePersona(true);
    }

    public void Rechazar()
    {
        if (!PuedeDecidir()) return;

        bool debePasar = DebePasar();   

        if (GameStatsManager.instance != null)
        {
            GameStatsManager.instance.RegistrarRechazar(!debePasar);
        }

        if (debePasar)
        {
            if (EconomyManager.instance != null)
            {
                EconomyManager.instance.RegistrarPenalizacion("Rechazo injustificado", EconomyManager.instance.multaRechazoInjustificado);
            }
        }

        SiguientePersona(false);
    }

    private bool PuedeDecidir()
    {
        if (personaActual == null)
        {
            return false;
        }

        if (carnetActual == null)
        {
            return false;
        }

        return true;
    }

    private bool DebePasar()
    {
        Alumno alumnoReal = personaActual.alumnoReal;

        if (personaActual.esColado) return false;

        if (!RostrosCoinciden(alumnoReal.rasgosFaciales, carnetActual.rasgosFaciales)) {
            return false;
        }

        if (!personaActual.vieneDeColaLibre)
        {
            if (carnetActual.turno > GameClock.TurnoGlobal){
                return false;
            }      
        }

        if (carnetActual.area != areaGlobal) {
            return false;
        }
        if (!CarnetVigente(carnetActual)) {
            return false;}

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

    private void SiguientePersona(bool fueAceptado)
    {
        Debug.Log("Siguiente persona...");
        personaActual = null;
        carnetActual = null;
        hayPersonaEnRevision = false;
        textoCola.text = "";

        if (queueCharacterManager != null)
        {
            queueCharacterManager.RetirarPersonaActual(fueAceptado);
        }

        if (QueueSystem.instance != null)
        {
            QueueSystem.instance.MandarSiguientePersona();
        }
        
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
        
        StopAllCoroutines();
    }

    private void MostrarFotoCarnet(CarnetData carnet)
    {
        if (carnet == null || carnet.rasgosFaciales == null)
        {
            return;
        }

        RasgosFaciales rasgos = carnet.rasgosFaciales;

        if (rasgos.esHombre)
        {
            AsignarSprite(cabezaFotoImage, cabezaHombre);
            AsignarSprite(narizFotoImage, narizHombre);

            AsignarSpritePorID(ojosFotoImage, ojosHombre, rasgos.ojosID);

            AsignarSpritePorID(cejasFotoImage, cejasHombre, rasgos.cejasID);

            AsignarSpritePorID(bocaFotoImage, bocasHombre, rasgos.bocaID);

            AsignarSpritePorID(cabelloFotoImage, cabellosHombre, rasgos.cabelloID);
        }

        else
        {
            AsignarSprite(cabezaFotoImage, cabezaMujer);
            AsignarSprite(narizFotoImage, narizMujer);

            AsignarSpritePorID(ojosFotoImage, ojosMujer, rasgos.ojosID);

            AsignarSpritePorID(cejasFotoImage, cejasMujer, rasgos.cejasID);

            AsignarSpritePorID(bocaFotoImage, bocasMujer, rasgos.bocaID);

            AsignarSpritePorID(cabelloFotoImage, cabellosMujer, rasgos.cabelloID);
        }

    }

    private void AsignarSpritePorID(Image imagen, Sprite[] opciones, int id)
    {
        if (opciones == null || id < 0 || id >= opciones.Length)
        {
            AsignarSprite(imagen, null);
            return;
        }

        AsignarSprite(imagen, opciones[id]);
    }

    private void AsignarSprite(Image imagen, Sprite sprite)
    {
        if (imagen == null)
        {
            return;
        }

        imagen.sprite = sprite;
        imagen.enabled = sprite != null;
    }

    private bool RostrosCoinciden(RasgosFaciales reales, RasgosFaciales carnet)
    {
        if (reales == null || carnet == null)
        {
            return false;
        }

        return reales.esHombre == carnet.esHombre &&
            reales.ojosID == carnet.ojosID &&
            reales.cejasID == carnet.cejasID &&
            reales.bocaID == carnet.bocaID &&
            reales.cabelloID == carnet.cabelloID;
    }
}