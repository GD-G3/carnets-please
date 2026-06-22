using UnityEngine;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
    [Header("Referencias")]
    public AlumnoGenerator generador;
    public RegistroDiario registroDiario;
    public RevisionManager revisionManager;

    [Header("Ajustes del Día")]
    public int cantidadAlumnosValidos = 10;
    public int cantidadImpostores = 3;

    // La cola de personas que van a pasar por la caseta hoy
    private Queue<Alumno> colaDeEntrada = new Queue<Alumno>();

    void Start()
    {
        GenerarDia();
        LlamarSiguientePersona();
    }

    public void GenerarDia()
    {
        if (registroDiario != null && registroDiario.lista_diaria != null)
        {
            registroDiario.lista_diaria.Clear();
        }
        else if (registroDiario != null)
        {
            registroDiario.lista_diaria = new List<Alumno>();
        }

        List<Alumno> todosLosQueEntran = new List<Alumno>();

        if (generador == null)
        {
            Debug.LogError("No hay generador de alumnos.");
            return;
        }

        // Generar alumnos válidos
        for (int i = 0; i < cantidadAlumnosValidos; i++)
        {
            Alumno alumnoValido = generador.GenerarAlumno();
            if (registroDiario != null)
            {
                registroDiario.lista_diaria.Add(alumnoValido);
            }
            todosLosQueEntran.Add(alumnoValido);
        }

        // Generar impostores
        for (int i = 0; i < cantidadImpostores; i++)
        {
            Alumno impostor = generador.GenerarAlumno();
            todosLosQueEntran.Add(impostor);
        }

        // Mezclar la lista
        MezclarLista(todosLosQueEntran);

        // Meter a la Cola
        foreach (var persona in todosLosQueEntran)
        {
            colaDeEntrada.Enqueue(persona);
        }
    }

    public void LlamarSiguientePersona()
    {
        if (colaDeEntrada.Count > 0)
        {
            Alumno personaEnVentanilla = colaDeEntrada.Dequeue();

            CarnetData carnetFisico = CrearAlumno(personaEnVentanilla);

            if (revisionManager != null)
            {
                revisionManager.RecibirDatosEscaneados(carnetFisico);
            }
        }
        else
        {
            Debug.Log("¡El día ha terminado! No hay más alumnos en la cola.");
        }
    }

    private CarnetData CrearAlumno(Alumno alumno)
    {
        CarnetData carnet = new CarnetData();
        carnet.nombreCompleto = alumno.nombreCompleto;
        carnet.codigo = alumno.codigo;
        carnet.fotoID = alumno.fotoID;
        carnet.facultad = alumno.facultad;
        carnet.carrera = alumno.carrera;

        carnet.turno = Random.Range(1, 4);
        carnet.area = Random.Range(1, 5);
        carnet.diaVencimiento = Random.Range(1, 30);
        carnet.mesVencimiento = Random.Range(1, 13);
        carnet.anioVencimiento = Random.Range(2024, 2028);

        return carnet;
    }

    private void MezclarLista(List<Alumno> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            Alumno temp = lista[i];
            int randomIndex = Random.Range(i, lista.Count);
            lista[i] = lista[randomIndex];
            lista[randomIndex] = temp;
        }
    }
}
