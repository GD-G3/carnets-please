using System.Collections;
using UnityEngine;

//Este script permite abrir una ventana 2D en Unity mediante un conjunto de sprites que representan el proceso de apertura. Se puede asignar un SpriteRenderer para mostrar los sprites, y opcionalmente se pueden reproducir sonidos durante la apertura.
public class WindowOpener2D : MonoBehaviour
{

    
    [Header("Window References")]
    public SpriteRenderer windowRenderer;

    [Header("Opening Sprites")]
    public Sprite[] openingSprites;

    [Header("Timing")]
    public float totalOpenDuration = 1.2f;

    [Header("Audio Optional")]
    public AudioSource audioSource;
    public AudioClip openStepClip;
    public AudioClip fullyOpenClip;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (windowRenderer == null)
        {
            windowRenderer = GetComponent<SpriteRenderer>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        SetClosed();
    }

    public void SetClosed()
    {
        IsOpen = false;

        if (openingSprites != null && openingSprites.Length > 0 && windowRenderer != null)
        {
            windowRenderer.sprite = openingSprites[0];
        }
    }

    public IEnumerator OpenWindowRoutine()
    {
        if (windowRenderer == null)
        {
            Debug.LogWarning("WindowOpener2D: falta SpriteRenderer.");
            yield break;
        }

        if (openingSprites == null || openingSprites.Length == 0)
        {
            Debug.LogWarning("WindowOpener2D: no hay sprites de apertura asignados.");
            yield break;
        }

        float secondsPerSprite = totalOpenDuration / openingSprites.Length;

        for (int i = 0; i < openingSprites.Length; i++)
        {
            windowRenderer.sprite = openingSprites[i];

            if (audioSource != null && openStepClip != null)
            {
                audioSource.PlayOneShot(openStepClip);
            }

            yield return new WaitForSeconds(secondsPerSprite);
        }

        IsOpen = true;

        if (audioSource != null && fullyOpenClip != null)
        {
            audioSource.PlayOneShot(fullyOpenClip);
        }
    }
}