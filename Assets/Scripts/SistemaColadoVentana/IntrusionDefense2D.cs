using UnityEngine;
//Donde se maneja la defensa de intrusiones en un entorno 2D, permitiendo a los jugadores interactuar con las intrusiones a través de clics o una tecla de acceso rápido.
public class IntrusionDefense2D : MonoBehaviour
{
    [Header("References")]
    public IntrusionManager2D manager;
    public Camera worldCamera;
    public Collider2D windowCollider;

    [Header("Input")]
    public LayerMask clickableLayers = -1;
    public bool requireActiveIntrusion = true;

    [Header("Keyboard Test")]
    public bool allowKeyboardFallback = true;
    public KeyCode fallbackKey = KeyCode.Space;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryClickDefense();
        }

        if (allowKeyboardFallback && Input.GetKeyDown(fallbackKey))
        {
            TryResolveIntrusion();
        }
    }

    private void TryClickDefense()
    {
        if (manager == null) return;

        if (worldCamera == null)
        {
            worldCamera = Camera.main;
        }

        if (worldCamera == null) return;

        Ray ray = worldCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, clickableLayers);

        if (hit.collider == null) return;

        if (windowCollider != null && hit.collider != windowCollider) return;

        TryResolveIntrusion();
    }

    public void TryResolveIntrusion()
    {
        if (manager == null) return;

        if (requireActiveIntrusion && !manager.IntrusionActive)
        {
            return;
        }

        manager.ResolveIntrusion();
    }
}