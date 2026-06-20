using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RevisionManager : MonoBehaviour
{
    public static RevisionManager instance;

    [Header("Carnet a revisar")]
    public CarnetData carnetActual;

    [Header("Referencias Adicionales")]
    public RegistroDiario registroDiario;

    [Header("Resultados")]
    public TMP_Text textoResultados;
    
    private int turnoActual = 1;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {

    }

    public void RecibirDatosEscaneados(CarnetData datos)
    {
        carnetActual = datos;

        if (textoResultados != null)
        {
            string mensajeCola = "NO PERTENECE A LA COLA";
            if (registroDiario != null && registroDiario.En_lista(datos.codigo))
            {
                mensajeCola = "PERTENECE A LA COLA";
            }
            else if (registroDiario == null)
            {
                mensajeCola = "(Falta asignar Registro)";
            }

            textoResultados.text = $"{datos.nombreCompleto.ObtenerNombreCompleto()}\nTurno {turnoActual}\n{mensajeCola}";
        }
        
        Debug.Log("Datos mostrados en la pantalla");
    }

    public void Permitir()
    {
        Debug.Log("Jugador: Permitir paso");
        SiguienteTurno();
    }

    public void Rechazar()
    {
        Debug.Log("Jugador: Rechazar paso");
        SiguienteTurno();
    }

    void SiguienteTurno()
    {
        turnoActual++;
        Debug.Log("Siguiente...");
        //colocar mas alumnos
        
        // Limpiar la UI para el próximo alumno (opcional)
        if (textoResultados != null) textoResultados.text = "";
    }
}