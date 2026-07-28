using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    private void Start()
    {
        EnsureGlobalManagers();
    }

    private void EnsureGlobalManagers()
    {
        if (MusicManager.instance == null && FindAnyObjectByType<MusicManager>() == null)
        {
            GameObject musicObj = new GameObject("MusicManager");
            musicObj.AddComponent<MusicManager>();
        }

        if (UISoundManager.instance == null && FindAnyObjectByType<UISoundManager>() == null)
        {
            GameObject uiSoundObj = new GameObject("UISoundManager");
            uiSoundObj.AddComponent<UISoundManager>();
        }

        if (SceneFader.instance == null && FindAnyObjectByType<SceneFader>() == null)
        {
            GameObject faderObj = new GameObject("SceneFader");
            faderObj.AddComponent<SceneFader>();
        }
    }

    public void CambiarPantalla(string Scene)
    {
        EnsureGlobalManagers();

        if (UISoundManager.instance != null)
        {
            UISoundManager.instance.PlayButtonClickSound();
        }

        if (SceneFader.instance != null)
        {
            SceneFader.instance.FadeToScene(Scene);
        }
        else
        {
            SceneManager.LoadScene(Scene);
        }
    }
}
