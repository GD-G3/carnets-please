using UnityEngine;
public class QuitGame : MonoBehaviour
{
    public void ExitGame(){
        Debug.Log("Saliendo del juego");
        Application.Quit();
    }
}
