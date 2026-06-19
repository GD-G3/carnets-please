using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Carnet : MonoBehaviour
{
    [Header("Datos mostrados en el carnet")]
    public CarnetData datos;

    private void OnMouseDown()
    {
        if (datos == null)
        {
            Debug.Log("Este carnet no tiene datos.");
            return;
        }

        Debug.Log($"Inspeccion de carnet: {datos.nombreCompleto.ObtenerNombreCompleto()}");
    }
}