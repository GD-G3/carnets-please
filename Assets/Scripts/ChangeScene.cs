using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    public void CambiarPantalla(string Scene){
        SceneManager.LoadScene(Scene);
    }
}
