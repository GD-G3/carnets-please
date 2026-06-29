using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Scanner : MonoBehaviour, IPointerDownHandler
{
    [Header("Referencias")]
    public RectTransform caja;

    private RectTransform rectTransform;
    private Canvas canvas;
    private bool isFollowingMouse = false;
    private bool ignoreNextClick = false;
    private Vector2 dragOffset;
    
    private Carnet carnetEscaneadoActual;
    private Vector2 startMousePos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
{
    if (!isFollowingMouse)
    {
        isFollowingMouse = true;
        ignoreNextClick = true;
        rectTransform.SetAsLastSibling();

        if (Mouse.current != null)
        {
            startMousePos = Mouse.current.position.ReadValue();

            RectTransform parentRect = rectTransform.parent as RectTransform;
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            Vector2 localMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                startMousePos,
                cam,
                out localMousePos
            );

            dragOffset = rectTransform.anchoredPosition - localMousePos;
        }
    }
}

    private void Update()
    {
        if (isFollowingMouse)
        {
            if (Mouse.current == null) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();

            // Seguimiento del mouse
            RectTransform parentRect = rectTransform.parent as RectTransform;
            Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                mousePos,
                cam,
                out localPoint
            );

            rectTransform.anchoredPosition = localPoint + dragOffset;

            // DETECCIÓN DEL CARNET 2D
            if (Vector2.Distance(startMousePos, mousePos) > 10f)
            {
                DetectarCarnet2D(mousePos);
            }

            // Detectar clic izquierdo para soltarlo en la caja
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (ignoreNextClick)
                {
                    ignoreNextClick = false;
                }
                else
                {
                    if (caja != null)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(caja, mousePos, cam))
                        {
                            isFollowingMouse = false; 
                            rectTransform.position = caja.position;
                            
                            // Al guardar el escáner, reseteamos el último carnet escaneado
                            carnetEscaneadoActual = null;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No has asignado la caja");
                    }
                }
            }
        }
    }

    private void DetectarCarnet2D(Vector2 mousePos)
    {
        if (Camera.main == null) return;

        Vector3 screenPos = new Vector3(mousePos.x, mousePos.y, Mathf.Abs(Camera.main.transform.position.z));
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        
        // Lanzamos un "rayo" para ver si hay un Collider2D en esa posición
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null)
        {
            Carnet carnet = hit.GetComponent<Carnet>();
            
            // Si es un carnet, está volteado y aún no lo hemos registrado
            if (carnet != null && carnet.isFlipped)
            {
                if (carnetEscaneadoActual != carnet)
                {
                    carnetEscaneadoActual = carnet;
                    RegistrarCarnet(carnet);
                }
            }
        }
        else
        {
            // Si quitamos el escáner de encima del carnet, permitimos que lo vuelva a escanear luego
            carnetEscaneadoActual = null;
        }
    }

    private void RegistrarCarnet(Carnet carnet)
    {
        if (RevisionManager.instance != null)
        {
            RevisionManager.instance.RecibirDatosEscaneados(carnet.datos);
        }
    }
}
