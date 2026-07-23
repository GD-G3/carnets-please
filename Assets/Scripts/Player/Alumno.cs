using UnityEngine;

[System.Serializable]
public class Alumno
{
    public NombreCompleto nombreCompleto;

    public string codigo;

    public RasgosFaciales rasgosFaciales;

    public int pantalonID;
    public int poloID;
    public int zapatosID;

    public string facultad;
    public string carrera;

    public string NombreTexto()
    {
        if (nombreCompleto == null)
        {
            return "SIN_NOMBRE";
        }

        return nombreCompleto.ObtenerNombreCompleto();
    }
}
