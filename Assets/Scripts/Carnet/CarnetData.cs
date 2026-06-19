using UnityEngine;

[System.Serializable]
public class CarnetData
{
    public NombreCompleto nombreCompleto;
    public string codigo;

    public int turno;
    public int area;
    public int fotoID;

    public string facultad;
    public string carrera;

    public int diaVencimiento;
    public int mesVencimiento;
    public int anioVencimiento;

    public string NombreTexto()
    {
        if (nombreCompleto == null)
        {
            return "SIN_NOMBRE";
        }

        return nombreCompleto.ObtenerNombreCompleto();
    }
}