using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] private float targetWidth = 1920f;
    [SerializeField] private float targetHeight = 1080f;

    void Start()
    {
        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scale = targetAspect / windowAspect;

        Camera cam = Camera.main;

        if (scale < 1.0f)
        {
            cam.orthographicSize = cam.orthographicSize / scale;
        }
    }
}
