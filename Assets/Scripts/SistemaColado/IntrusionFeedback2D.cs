using System.Collections;
using UnityEngine;
// Este script define una clase IntrusionFeedback2D que proporciona retroalimentación visual y auditiva para eventos de intrusión en un juego 2D. Permite reproducir clips de audio específicos para diferentes eventos (intrusión, completado, resuelto) y también incluye una función para sacudir la cámara, mejorando la inmersión del jugador durante situaciones de intrusión.
public class IntrusionFeedback2D : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip intrusionClip;
    public AudioClip completeClip;
    public AudioClip resolvedClip;

    [Header("Camera Shake")]
    public Camera cameraToShake;
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.08f;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (cameraToShake == null)
        {
            cameraToShake = Camera.main;
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    public void PlayIntrusionFeedback()
    {
        PlayClip(intrusionClip);
        ShakeCamera();
    }

    public void PlayCompleteFeedback()
    {
        PlayClip(completeClip);
    }

    public void PlayResolvedFeedback()
    {
        PlayClip(resolvedClip);
    }

    public void ShakeCamera()
    {
        if (cameraToShake == null) return;

        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 originalPosition = cameraToShake.transform.localPosition;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;

            Vector2 offset = Random.insideUnitCircle * shakeStrength;

            cameraToShake.transform.localPosition =
                originalPosition + new Vector3(offset.x, offset.y, 0f);

            yield return null;
        }

        cameraToShake.transform.localPosition = originalPosition;
        shakeCoroutine = null;
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.PlayOneShot(clip);
    }
}