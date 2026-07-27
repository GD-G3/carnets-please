using System;
using UnityEngine;
using UnityEngine.UI;

public class QueueCharacterUI : MonoBehaviour
{
    [Header("Imágenes UI")]
    public Image cuerpoImage;
    public Image cabezaImage;
    public Image bocaImage;
    public Image cabelloImage;
    public Image cejasImage;
    public Image narizImage;
    public Image ojosImage;
    public Image pantalonImage;
    public Image poloImage;
    public Image zapatosImage;

    [Header("Partes fijas de hombre")]
    public ParteAnimadaUI cuerpoHombre;
    public ParteAnimadaUI cabezaHombre;
    public ParteAnimadaUI narizHombre;

    [Header("Partes fijas de mujer")]
    public ParteAnimadaUI cuerpoMujer;
    public ParteAnimadaUI cabezaMujer;
    public ParteAnimadaUI narizMujer;

    [Header("Opciones aleatorias de hombre")]
    public ParteAnimadaUI[] bocasHombre;
    public ParteAnimadaUI[] cabellosHombre;
    public ParteAnimadaUI[] cejasHombre;
    public ParteAnimadaUI[] ojosHombre;
    public ParteAnimadaUI[] pantalonesHombre;
    public ParteAnimadaUI[] polosHombre;
    public ParteAnimadaUI[] zapatosHombre;

    [Header("Opciones aleatorias de mujer")]
    public ParteAnimadaUI[] bocasMujer;
    public ParteAnimadaUI[] cabellosMujer;
    public ParteAnimadaUI[] cejasMujer;
    public ParteAnimadaUI[] ojosMujer;
    public ParteAnimadaUI[] pantalonesMujer;
    public ParteAnimadaUI[] polosMujer;
    public ParteAnimadaUI[] zapatosMujer;

    //No aleatorio
    private ParteAnimadaUI cuerpoSeleccionado;
    private ParteAnimadaUI cabezaSeleccionada;
    private ParteAnimadaUI narizSeleccionada;

    //Aleatorio
    private ParteAnimadaUI bocaSeleccionada;
    private ParteAnimadaUI cabelloSeleccionado;
    private ParteAnimadaUI cejasSeleccionadas;
    private ParteAnimadaUI ojosSeleccionados;
    private ParteAnimadaUI pantalonSeleccionado;
    private ParteAnimadaUI poloSeleccionado;
    private ParteAnimadaUI zapatosSeleccionados;

    public void ConfigurarDesdePersona(PersonaEnCola persona)
    {
        if (persona == null || persona.alumnoReal == null)
        {
            Debug.LogWarning("No se recibió una persona válida.");
            return;
        }

        Alumno alumno = persona.alumnoReal;

        bool esHombre = alumno.nombreCompleto.genero == Genero.Hombre;

        if (esHombre)
        {
            ConfigurarHombre(alumno);
        }
        else
        {
            ConfigurarMujer(alumno);
        }

        MostrarQuieto();
    }

    private void ConfigurarHombre(Alumno alumno)
    {
        RasgosFaciales rasgos = alumno.rasgosFaciales;
        
        // Partes fijas
        cuerpoSeleccionado = cuerpoHombre;
        cabezaSeleccionada = cabezaHombre;
        narizSeleccionada = narizHombre;

        // Partes aleatorias
        bocaSeleccionada = bocasHombre[rasgos.bocaID];
        cabelloSeleccionado = cabellosHombre[rasgos.cabelloID];
        cejasSeleccionadas = cejasHombre[rasgos.cejasID];
        ojosSeleccionados = ojosHombre[rasgos.ojosID];
        pantalonSeleccionado = pantalonesHombre[alumno.pantalonID];
        poloSeleccionado = polosHombre[alumno.poloID];
        zapatosSeleccionados = zapatosHombre[alumno.zapatosID];
    }

    private void ConfigurarMujer(Alumno alumno)
    {
        RasgosFaciales rasgos = alumno.rasgosFaciales;
        // Partes fijas
        cuerpoSeleccionado = cuerpoMujer;
        cabezaSeleccionada = cabezaMujer;
        narizSeleccionada = narizMujer;

        // Partes aleatorias
        bocaSeleccionada = bocasMujer[rasgos.bocaID];
        cabelloSeleccionado = cabellosMujer[rasgos.cabelloID];
        cejasSeleccionadas = cejasMujer[rasgos.cejasID];
        ojosSeleccionados = ojosMujer[rasgos.ojosID];
        pantalonSeleccionado = pantalonesMujer[alumno.pantalonID];
        poloSeleccionado = polosMujer[alumno.poloID];
        zapatosSeleccionados = zapatosMujer[alumno.zapatosID];
    }

    public void MostrarQuieto()
    {
        AsignarSprite(cuerpoImage, cuerpoSeleccionado?.spriteQuieto);
        AsignarSprite(cabezaImage, cabezaSeleccionada?.spriteQuieto);
        AsignarSprite(narizImage, narizSeleccionada?.spriteQuieto);

        AsignarSprite(bocaImage, bocaSeleccionada?.spriteQuieto);
        AsignarSprite(cabelloImage, cabelloSeleccionado?.spriteQuieto);
        AsignarSprite(cejasImage, cejasSeleccionadas?.spriteQuieto);
        AsignarSprite(ojosImage, ojosSeleccionados?.spriteQuieto);
        AsignarSprite(pantalonImage, pantalonSeleccionado?.spriteQuieto);
        AsignarSprite(poloImage, poloSeleccionado?.spriteQuieto);
        AsignarSprite(zapatosImage, zapatosSeleccionados?.spriteQuieto);
    }

    public void MostrarFrameCaminar(int frame)
    {
        MostrarFrame(cuerpoImage, cuerpoSeleccionado, frame);
        MostrarFrame(cabezaImage, cabezaSeleccionada, frame);
        MostrarFrame(narizImage, narizSeleccionada, frame);

        MostrarFrame(bocaImage, bocaSeleccionada, frame);
        MostrarFrame(cabelloImage, cabelloSeleccionado, frame);
        MostrarFrame(cejasImage, cejasSeleccionadas, frame);
        MostrarFrame(ojosImage, ojosSeleccionados, frame);
        MostrarFrame(pantalonImage, pantalonSeleccionado, frame);
        MostrarFrame(poloImage, poloSeleccionado, frame);
        MostrarFrame(zapatosImage, zapatosSeleccionados, frame);
    }

    private void MostrarFrame(Image image, ParteAnimadaUI parte, int frame)
    {
        if (parte == null)
        {
            AsignarSprite(image, null);
            return;
        }

        AsignarSprite(image, parte.ObtenerFrame(frame));
    }

    private void AsignarSprite(Image image, Sprite sprite)
    {
        if (image == null)
        {
            return;
        }

        image.sprite = sprite;
        image.enabled = sprite != null;
    }

    public void MirarIzquierda(bool izquierda)
    {
        float escala = Mathf.Abs(transform.localScale.x);

        if (izquierda)
        {
            transform.localScale = new Vector3(-escala, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(escala, transform.localScale.y, transform.localScale.z);
        }
    }
}