using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject deskPanel;
    public GameObject cameraPanel;

    public void OpenCameraPanel()
    {
        deskPanel.SetActive(false);
        cameraPanel.SetActive(true);
    }

    public void CloseCameraPanel()
    {
        cameraPanel.SetActive(false);
        deskPanel.SetActive(true);
    }
}