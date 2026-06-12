using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Carnet : MonoBehaviour
{
    [Header("Datos del alumno")]
    public Alumno datos;

    private void OnMouseDown()
    {
        Debug.Log("Inspección de carnet {datos.nombre}");
    }
}
