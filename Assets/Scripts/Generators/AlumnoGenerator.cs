using UnityEngine;
using System.Collections.Generic;

public class AlumnoGenerator : MonoBehaviour
{
    [Header("Generador de nombres")]
    public NombreGenerator nombreGenerator;

    [Header("Facultades y carreras")]
    public List<FacultadData> facultades;

    [Header("Cantidad de opciones faciales")]
    public int cantidadOjosHombre = 3;
    public int cantidadCejasHombre = 3;
    public int cantidadBocasHombre = 3;
    public int cantidadCabellosHombre = 3;

    public int cantidadOjosMujer = 3;
    public int cantidadCejasMujer = 3;
    public int cantidadBocasMujer = 3;
    public int cantidadCabellosMujer = 3;

    [Header("Cantidad de opciones ropa")]
    public int cantidadPantalonesHombre = 3;
    public int cantidadPolosHombre = 3;
    public int cantidadZapatosHombre = 3;

    public int cantidadPantalonesMujer = 3;
    public int cantidadPolosMujer = 3;
    public int cantidadZapatosMujer = 3;

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

        bool esHombre = alumno.nombreCompleto.genero == Genero.Hombre;
        alumno.rasgosFaciales = GenerarRasgosFaciales(esHombre);

        if (esHombre)
        {
            alumno.pantalonID = UnityEngine.Random.Range(0, cantidadPantalonesHombre);
            alumno.poloID = UnityEngine.Random.Range(0, cantidadPolosHombre);
            alumno.zapatosID = UnityEngine.Random.Range(0, cantidadZapatosHombre);
        }
        else
        {
            alumno.pantalonID = UnityEngine.Random.Range(0, cantidadPantalonesMujer);
            alumno.poloID = UnityEngine.Random.Range(0, cantidadPolosMujer);
            alumno.zapatosID = UnityEngine.Random.Range(0, cantidadZapatosMujer);
        }


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

    public RasgosFaciales GenerarRasgosFaciales(bool esHombre)
    {
        RasgosFaciales rasgos = new RasgosFaciales();
        rasgos.esHombre = esHombre;

        rasgos.cabezaID = 0;
        rasgos.narizID = 0;

        if (esHombre)
        {
            rasgos.ojosID = UnityEngine.Random.Range(0, cantidadOjosHombre);
            rasgos.cejasID = UnityEngine.Random.Range(0, cantidadCejasHombre);
            rasgos.bocaID = UnityEngine.Random.Range(0, cantidadBocasHombre);
            rasgos.cabelloID = UnityEngine.Random.Range(0, cantidadCabellosHombre);
        }
        else
        {
            rasgos.ojosID = UnityEngine.Random.Range(0, cantidadOjosMujer);
            rasgos.cejasID = UnityEngine.Random.Range(0, cantidadCejasMujer);
            rasgos.bocaID = UnityEngine.Random.Range(0, cantidadBocasMujer);
            rasgos.cabelloID = UnityEngine.Random.Range(0, cantidadCabellosMujer);
        }

        return rasgos;
    }
}