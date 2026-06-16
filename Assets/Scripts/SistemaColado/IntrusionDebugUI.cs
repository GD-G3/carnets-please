using UnityEngine;
using TMPro;

public class IntrusionDebugUI : MonoBehaviour
{
    public IntrusionManager2D intrusionManager;
    public TMP_Text debugText;

    private void Update()
    {
        if (intrusionManager == null || debugText == null) return;

        debugText.text =
            $"Probabilidad: {intrusionManager.CurrentProbability * 100f:F2}%\n" +
            $"Tiempo: {intrusionManager.ElapsedTime:F1}s\n" +
            $"Intrusión activa: {intrusionManager.IntrusionActive}";
    }
}