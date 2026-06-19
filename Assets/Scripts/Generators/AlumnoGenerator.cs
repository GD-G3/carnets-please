using UnityEngine;
using System.Collections.Generic;

public class AlumnoGenerator : MonoBehaviour
{
    [Header("Generador de nombres")]
    public NombreGenerator nombreGenerator;

    [Header("Facultades y carreras")]
    public List<FacultadData> facultades;

    public Alumno GenerarAlumno()
    {
        Genero genero = GenerarGenero();
        return GenerarAlumno(genero);
    }

    public Alumno GenerarAlumno(Genero genero)
    {
        Alumno alumno = new Alumno();

        alumno.nombreCompleto = nombreGenerator.GenerarNombreCompleto(genero);
        alumno.codigo = GenerarCodigo();
        alumno.fotoID = GenerarFotoID(genero);

        AsignarFacultadYCarrera(alumno);

        return alumno;
    }

    private Genero GenerarGenero()
    {
        int random = UnityEngine.Random.Range(0, 2);

        if (random == 0)
        {
            return Genero.Hombre;
        }

        return Genero.Mujer;
    }

    private void AsignarFacultadYCarrera(Alumno alumno)
    {
        if (facultades == null || facultades.Count == 0)
        {
            Debug.LogError("No hay facultades registradas.");
            alumno.facultad = "SIN_FACULTAD";
            alumno.carrera = "SIN_CARRERA";
            return;
        }

        FacultadData facultadElegida = facultades[UnityEngine.Random.Range(0, facultades.Count)];

        alumno.facultad = facultadElegida.nombreFacultad;

        if (facultadElegida.carreras == null || facultadElegida.carreras.Count == 0)
        {
            Debug.LogError("La facultad " + facultadElegida.nombreFacultad + " no tiene carreras.");
            alumno.carrera = "SIN_CARRERA";
            return;
        }

        int indexCarrera = UnityEngine.Random.Range(0, facultadElegida.carreras.Count);
        alumno.carrera = facultadElegida.carreras[indexCarrera];
    }

    private string GenerarCodigo()
    {
        int codigo = UnityEngine.Random.Range(20200000, 20270000);
        return codigo.ToString();
    }

    private int GenerarFotoID(Genero g)
    {
        if (g == Genero.Hombre)
        {
            return UnityEngine.Random.Range(1, 11);
        }
        else
        {
            return UnityEngine.Random.Range(11, 21);
        }
        
    }

}