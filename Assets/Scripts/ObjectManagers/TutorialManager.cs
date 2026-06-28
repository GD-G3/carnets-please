using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    public GameObject panelTutorial;
    public TextMeshProUGUI textoDialogo;

    [TextArea]
    public string[] dialogosDia1;
    private int indiceDialogoActual = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panelTutorial != null)
        {
            panelTutorial.SetActive(false);
        }

        if (DayManager.instance != null)
        {
            DayManager.instance.OnDayStarted += RevisarTutorialDia;
        }
    }

    private void OnDestroy()
    {
        if (DayManager.instance != null)
        {
            DayManager.instance.OnDayStarted -= RevisarTutorialDia;
        }
    }

    private void RevisarTutorialDia(int dia)
    {
        if (dia == 1)
        {
            IniciarTutorial();
        }
    }

    public void IniciarTutorial()
    {
        if (panelTutorial == null) return;
        
        indiceDialogoActual = 0;
        panelTutorial.SetActive(true);
        MostrarSiguienteDialogo();
    }

    public void MostrarSiguienteDialogo()
    {
        if (dialogosDia1 == null || dialogosDia1.Length == 0)
        {
            TerminarTutorial();
            return;
        }

        if (indiceDialogoActual < dialogosDia1.Length)
        {
            textoDialogo.text = dialogosDia1[indiceDialogoActual];
            indiceDialogoActual++;
        }
        else
        {
            TerminarTutorial();
        }
    }

    private void TerminarTutorial()
    {
        if (panelTutorial != null)
        {
            panelTutorial.SetActive(false);
        }
        Debug.Log("Tutorial terminado.");
    }
}
