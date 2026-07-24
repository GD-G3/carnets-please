using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject deskPanel;
    public GameObject cameraPanel;

    public void OpenCameraPanel()
    {
        cameraPanel.SetActive(true);
    }

    public void CloseCameraPanel()
    {
        cameraPanel.SetActive(false);
    }
}