using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class IntrusionManager2D : MonoBehaviour
{
    [Header("Probability Settings")]
    [Range(0f, 1f)]
    public float baseProbability = 0.02f;

    [Min(0f)]
    public float maxRampMultiplier = 4f;

    [Min(0.01f)]
    public float maxRampTime = 60f;

    [Min(0.01f)]
    public float tickInterval = 0.35f;

    [Header("Scene References")]
    public Transform windowSpawnPoint;
    public Transform windowEntryTarget;
    public GameObject intruderPrefab;

    [Min(0.01f)]
    public float entryDuration = 2.5f;

    [Header("Window Opening")]
    public WindowOpener2D windowOpener;
    public bool openWindowBeforeEntry = true;

    [Header("Animation Triggers")]
    public string enterTrigger = "EnterWindow";
    public string idleTrigger = "Idle";

    [Header("Resolve Settings")]
    public bool resetTimerOnResolve = true;
    public bool destroyIntruderOnResolve = true;

    [Header("Events")]
    public UnityEvent OnIntrusionTriggered;
    public UnityEvent OnIntrusionComplete;
    public UnityEvent OnIntrusionResolved;

    private float elapsedTime = 0f;
    private float tickTimer = 0f;
    private bool intrusionActive = false;
    private GameObject currentIntruder;
    private Coroutine moveCoroutine;

    public float ElapsedTime => elapsedTime;
    public bool IntrusionActive => intrusionActive;
    public float CurrentProbability => GetCurrentProbability();

    private void Update()
    {
        if (intrusionActive) return;

        elapsedTime += Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            EvaluateTick();
        }
    }

    private void EvaluateTick()
    {
        float probability = GetCurrentProbability();
        float roll = Random.value;

        if (roll < probability)
        {
            TriggerIntrusion();
        }
    }

    public void TriggerIntrusion()
    {
        if (intrusionActive) return;

        if (intruderPrefab == null || windowSpawnPoint == null || windowEntryTarget == null)
        {
            Debug.LogWarning("IntrusionManager2D: faltan referencias en el Inspector.");
            return;
        }

        intrusionActive = true;

        currentIntruder = Instantiate(
            intruderPrefab,
            windowSpawnPoint.position,
            windowSpawnPoint.rotation
        );

        currentIntruder.SetActive(true);

        Intruder2D intruder = currentIntruder.GetComponent<Intruder2D>();
        intruder?.OnSpawned();

        Animator animator = currentIntruder.GetComponent<Animator>();
        SetAnimatorTrigger(animator, enterTrigger);

        OnIntrusionTriggered?.Invoke();

        moveCoroutine = StartCoroutine(IntrusionSequence());
    }
    private IEnumerator IntrusionSequence()
    {
        if (openWindowBeforeEntry && windowOpener != null)
        {
            yield return StartCoroutine(windowOpener.OpenWindowRoutine());
        }

        yield return StartCoroutine(MoveIntruder());
    }
    private IEnumerator MoveIntruder()
    {
        float timer = 0f;

        Vector3 start = currentIntruder.transform.position;
        Vector3 end = windowEntryTarget.position;

        end.z = start.z;

        while (timer < entryDuration)
        {
            if (currentIntruder == null) yield break;

            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / entryDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            Vector3 newPosition = Vector3.Lerp(start, end, smoothT);
            newPosition.z = start.z;

            currentIntruder.transform.position = newPosition;

            yield return null;
        }

        if (currentIntruder != null)
        {
            currentIntruder.transform.position = end;

            Animator animator = currentIntruder.GetComponent<Animator>();
            SetAnimatorTrigger(animator, idleTrigger);

            Intruder2D intruder = currentIntruder.GetComponent<Intruder2D>();
            intruder?.OnReachedRoom();

            if (EconomyManager.instance != null)
            {
                EconomyManager.instance.RegistrarPenalizacion("Intruso por la ventana", EconomyManager.instance.multaIntrusoVentana);
            }
        }

        OnIntrusionComplete?.Invoke();

        moveCoroutine = null;
    }

    public void ResolveIntrusion()
    {
        if (!intrusionActive) return;

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        if (currentIntruder != null)
        {
            Intruder2D intruder = currentIntruder.GetComponent<Intruder2D>();
            intruder?.OnResolved();

            if (destroyIntruderOnResolve)
            {
                Destroy(currentIntruder);
            }
        }

        currentIntruder = null;
        intrusionActive = false;
        tickTimer = 0f;

        if (resetTimerOnResolve)
        {
            elapsedTime = 0f;
        }

        OnIntrusionResolved?.Invoke();
    }

    public float GetCurrentProbability()
    {
        float t = Mathf.Clamp01(elapsedTime / maxRampTime);
        float probability = baseProbability * Mathf.Lerp(1f, maxRampMultiplier, t);

        return Mathf.Clamp01(probability);
    }

    private void SetAnimatorTrigger(Animator animator, string triggerName)
    {
        if (animator == null) return;
        if (string.IsNullOrWhiteSpace(triggerName)) return;
        if (animator.runtimeAnimatorController == null) return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == triggerName &&
                parameter.type == AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(triggerName);
                return;
            }
        }
    }
}
