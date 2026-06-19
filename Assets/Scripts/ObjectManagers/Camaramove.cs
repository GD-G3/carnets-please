using System.Collections;
using UnityEngine;

public class CameraPanImage : MonoBehaviour
{
    [Header("Posiciones")]
    public float startX = 146.63f;
    public float endX = -145f;
    public float posY = 31.372f;

    [Header("Tiempo")]
    public float moveTime = 5f;
    public float waitTime = 1f;

    private RectTransform rectTransform;
    private Coroutine moveCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // Cada vez que se abre la cámara, empieza desde la posición inicial
        rectTransform.anchoredPosition = new Vector2(startX, posY);

        moveCoroutine = StartCoroutine(MoveLoop());
    }

    private void OnDisable()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    private IEnumerator MoveLoop()
    {
        while (true)
        {
            yield return MoveTo(endX);
            yield return new WaitForSeconds(waitTime);

            yield return MoveTo(startX);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveTo(float targetX)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(targetX, posY);

        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / moveTime;
            t = Mathf.SmoothStep(0f, 1f, t);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
    }
}