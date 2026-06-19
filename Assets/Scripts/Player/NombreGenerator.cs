using UnityEngine;
using System.Collections.Generic;

public enum Genero
{
    Hombre,
    Mujer
}

[System.Serializable]
public class NombreCompleto
{
    public string nombre;
    public string apellidoPaterno;
    public string apellidoMaterno;
    public Genero genero;

    public string ObtenerNombreCompleto()
    {
        return nombre + " " + apellidoPaterno + " " + apellidoMaterno;
    }
}

public class NombreGenerator : MonoBehaviour
{
    [Header("Nombres")]
    public List<string> nombresHombre = new List<string>();
    public List<string> nombresMujer = new List<string>();

    [Header("Apellidos")]
    public List<string> apellidos = new List<string>();

    public NombreCompleto GenerarNombreCompleto(Genero g)
    {
        NombreCompleto nombreCompleto = new NombreCompleto();

        nombreCompleto.genero = g;

        if (g == Genero.Hombre)
        {
            nombreCompleto.nombre = ElegirAleatorio(nombresHombre);
        }
        else
        {
            nombreCompleto.nombre = ElegirAleatorio(nombresMujer);
        }

        nombreCompleto.apellidoPaterno = ElegirAleatorio(apellidos);
        nombreCompleto.apellidoMaterno = ElegirAleatorio(apellidos);

        return nombreCompleto;
    }

    private string ElegirAleatorio(List<string> lista)
    {
        if (lista == null || lista.Count == 0)
        {
            Debug.LogError("Una lista de nombres o apellidos está vacía.");
            return "SIN_DATO";
        }

        int index = UnityEngine.Random.Range(0, lista.Count);
        return lista[index];
    }
}
