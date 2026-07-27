using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(QueueCharacterUI))]
public class QueueCharacterMovementUI : MonoBehaviour
{
    [Header("Movimiento")]
    public float duracionMovimiento = 3f;

    [Header("Animación de caminata")]
    public float framesPorSegundo = 18f;

    [Header("Suavizado")]
    public bool usarMovimientoSuavizado = false;

    private RectTransform rectTransform;
    private QueueCharacterUI personajeVisual;
    private Coroutine movimientoActual;

    public bool EstaMoviendose =>
        movimientoActual != null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        personajeVisual = GetComponent<QueueCharacterUI>();
    }

    public void ColocarEn(RectTransform punto)
    {
        if (punto == null)
        {
            Debug.LogWarning(
                "No se asignó el punto inicial."
            );
            return;
        }

        rectTransform.anchoredPosition = punto.anchoredPosition;
    }

    public void MoverHacia(RectTransform puntoDestino, bool mirarIzquierda = false, Action alFinalizar = null)
    {
        if (puntoDestino == null)
        {
            Debug.LogWarning(
                "No se asignó el punto de destino."
            );
            return;
        }
        personajeVisual.MirarIzquierda(mirarIzquierda);

        if (movimientoActual != null)
        {
            StopCoroutine(movimientoActual);
        }

        movimientoActual = StartCoroutine( RutinaMover(puntoDestino.anchoredPosition, alFinalizar) );
    }

    private IEnumerator RutinaMover(Vector2 destino, Action alFinalizar)
    {
        Vector2 origen = rectTransform.anchoredPosition;

        float tiempoMovimiento = 0f;
        float tiempoAnimacion = 0f;

        int ultimoFrameMostrado = -1;

        while (tiempoMovimiento < duracionMovimiento)
        {
            float delta = Time.deltaTime;

            tiempoMovimiento += delta;
            tiempoAnimacion += delta;

            float progreso = Mathf.Clamp01(
                tiempoMovimiento / duracionMovimiento
            );

            float progresoMovimiento =
                usarMovimientoSuavizado
                    ? Mathf.SmoothStep(0f, 1f, progreso)
                    : progreso;

            rectTransform.anchoredPosition =
                Vector2.Lerp(
                    origen,
                    destino,
                    progresoMovimiento
                );

            int numeroFrame =
                Mathf.FloorToInt(
                    tiempoAnimacion * framesPorSegundo
                );

            if (numeroFrame != ultimoFrameMostrado)
            {
                ultimoFrameMostrado = numeroFrame;

                personajeVisual.MostrarFrameCaminar(
                    numeroFrame
                );
            }

            yield return null;
        }

        rectTransform.anchoredPosition = destino;

        personajeVisual.MostrarQuieto();

        movimientoActual = null;

        alFinalizar?.Invoke();
    }

    public void DetenerMovimiento()
    {
        if (movimientoActual != null)
        {
            StopCoroutine(movimientoActual);
            movimientoActual = null;
        }

        if (personajeVisual != null)
        {
            personajeVisual.MostrarQuieto();
        }
    }
}