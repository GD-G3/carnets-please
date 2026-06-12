using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RevisionManager : MonoBehaviour
{
    public static RevisionManager instance;

    [Header("Lista del dia")]
    public List<Alumno> lista;

    [Header("Interfaz")]
    public TextMeshProUGUI hojaDia;

    [Header("Carnet a revisar")]
    public Carnet carnetActual;

    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CrearLista();
    }

    void CrearLista()
    {
        hojaDia.text = "Alumnos Autorizados Hoy:\n";

        foreach (Alumno alumno in lista)
        {
            hojaDia.text += $"- {alumno.nombre} | código: {alumno.codigo}\n";
        }
    }

 /* 
    nombre;
    codigo;
    fecha_vencimiento;
    foto;
    facultad;
    carrera;
*/

    bool Validar()
    {
        if(carnetActual == null) return false;

        Alumno datosCarnet = carnetActual.datos;

        foreach (Alumno oficial in lista)
        {
            if(oficial.codigo == datosCarnet.codigo &&
               oficial.fecha_vencimiento == datosCarnet.fecha_vencimiento &&
               oficial.foto == datosCarnet.foto &&
               oficial.facultad == datosCarnet.facultad &&
               oficial.carrera == datosCarnet.carrera &&
               oficial.nombre == datosCarnet.nombre)
            { 
                return true;
            }
        }
        return false;
    }

    public void Permitir()
    {
        if(Validar())
        {
            Debug.Log("Excelente!");
        }
        else
        {
            Debug.Log("Error");
        }
        SiguienteTurno();
    }

    public void Rechazar()
    {
        if (!Validar())
        {
            Debug.Log("Excelente!");
        }
        else
        {
            Debug.Log("Error");
        }
        SiguienteTurno();
    }

    void SiguienteTurno()
    {
        Debug.Log("Siguiente...");
        //colocar mas alumnos
    }
}
