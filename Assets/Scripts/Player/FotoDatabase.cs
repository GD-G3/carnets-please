using UnityEngine;
using System.Collections.Generic;

public class FotoDatabase : MonoBehaviour
{
    public List<FotoData> fotos = new List<FotoData>();

    public FotoData ObtenerFotoAleatoria(Genero genero)
    {
        List<FotoData> fotosFiltradas = new List<FotoData>();

        foreach (FotoData foto in fotos)
        {
            if (foto.genero == genero)
            {
                fotosFiltradas.Add(foto);
            }
        }

        if (fotosFiltradas.Count == 0)
        {
            Debug.LogError("No hay fotos registradas para el genero: " + genero);
            return null;
        }

        int index = UnityEngine.Random.Range(0, fotosFiltradas.Count);
        return fotosFiltradas[index];
    }

    public FotoData ObtenerFotoPorID(int id)
    {
        foreach (FotoData foto in fotos)
        {
            if (foto.id == id)
            {
                return foto;
            }
        }

        Debug.LogWarning("No se encontro una foto con ID: " + id);
        return null;
    }

    public FotoData ObtenerFotoDistinta(int idActual, Genero genero)
    {
        List<FotoData> opciones = new List<FotoData>();

        foreach (FotoData foto in fotos)
        {
            if (foto.genero == genero && foto.id != idActual)
            {
                opciones.Add(foto);
            }
        }

        if (opciones.Count == 0)
        {
            Debug.LogWarning("No hay una foto distinta disponible para el genero: " + genero);
            return null;
        }

        int index = UnityEngine.Random.Range(0, opciones.Count);
        return opciones[index];
    }
}