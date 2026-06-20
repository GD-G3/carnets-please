using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Carnet : MonoBehaviour
{
    [Header("Datos mostrados en el carnet")]
    public CarnetData datos;

    [Header("Imágenes (Volteo)")]
    public Sprite spriteFrontal;
    public Sprite spriteTrasero;

    public bool isFlipped { get; private set; } = false;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        // Evita que el clic traspase si hay UI tapando el objeto
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        VoltearCarnet();
    }

    private void VoltearCarnet()
    {
        isFlipped = !isFlipped;
        
        spriteRenderer.sprite = isFlipped ? spriteTrasero : spriteFrontal;

        if (isFlipped)
        {
            Debug.Log("El carnet está volteado. Puedes pasar el escáner para registrarlo.");
        }
        else
        {
            Debug.Log("Carnet de frente.");
        }
    }
}