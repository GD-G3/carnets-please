using UnityEngine;
using System.Collections.Generic;
public class RegistroDiario : MonoBehaviour
{
    public List<Alumno> lista_diaria;
    
    public bool En_lista(Alumno datos)
    {
        foreach (var alumno in lista_diaria)
        {
            // verificacion por código
            if (alumno.codigo == datos.codigo)
            {
                return true;
            }
        }

        Debug.Log("Código no está en lista");
        return false;
    }
}
