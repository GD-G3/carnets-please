using UnityEngine;

[System.Serializable]
public class CarnetData
{
    public NombreCompleto nombreCompleto;
    public string codigo;

    public int turno;
    public int area;

    public string facultad;
    public string carrera;

    public int diaVencimiento;
    public int mesVencimiento;
    public int anioVencimiento;

    public RasgosFaciales rasgosFaciales;

    public string NombreTexto()
    {
        if (nombreCompleto == null)
        {
            return "SIN_NOMBRE";
        }

        return nombreCompleto.ObtenerNombreCompleto();
    }
}