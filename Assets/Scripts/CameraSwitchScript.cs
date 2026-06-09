using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    public Image cameraImage;

    public Sprite camera1Sprite;
    public Sprite camera2Sprite;

    private int currentCamera = 0;

    private void Start()
    {
        ShowCamera1();
    }

    public void NextCamera()
    {
        if (currentCamera == 0)
        {
            ShowCamera2();
        }
        else
        {
            ShowCamera1();
        }
    }

    private void ShowCamera1()
    {
        currentCamera = 0;
        cameraImage.sprite = camera1Sprite;
        //sprites de estudiantes despues de añadira
    }

    private void ShowCamera2()
    {
        currentCamera = 1;
        cameraImage.sprite = camera2Sprite;
    }
}
