using UnityEngine;
// Este script define una clase Intruder2D que representa un intruso en un juego 2D. La clase requiere componentes como SpriteRenderer, Collider2D, Rigidbody2D y AudioSource para funcionar correctamente. Proporciona métodos para manejar eventos como el spawn, alcanzar una habitación y resolver la situación, reproduciendo clips de audio opcionales en cada caso.
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]
public class Intruder2D : MonoBehaviour
{
    [Header("Optional Audio")]
    public AudioClip spawnClip;
    public AudioClip reachedRoomClip;
    public AudioClip resolvedClip;

    private SpriteRenderer spriteRenderer;
    private Collider2D collider2DComponent;
    private Rigidbody2D rigidbody2DComponent;
    private AudioSource audioSource;

    private void Reset()
    {
        CacheComponents();
        ConfigureFor2D();
    }

    private void Awake()
    {
        CacheComponents();
        ConfigureFor2D();
    }

    private void CacheComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2DComponent = GetComponent<Collider2D>();
        rigidbody2DComponent = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void ConfigureFor2D()
    {
        if (rigidbody2DComponent != null)
        {
            rigidbody2DComponent.bodyType = RigidbodyType2D.Kinematic;
            rigidbody2DComponent.gravityScale = 0f;
            rigidbody2DComponent.freezeRotation = true;
        }

        if (collider2DComponent != null)
        {
            collider2DComponent.isTrigger = true;
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 5;
        }
    }

    public void OnSpawned()
    {
        PlayClip(spawnClip);
    }

    public void OnReachedRoom()
    {
        PlayClip(reachedRoomClip);
    }

    public void OnResolved()
    {
        if (resolvedClip != null)
        {
            AudioSource.PlayClipAtPoint(resolvedClip, transform.position);
        }
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.PlayOneShot(clip);
    }
}