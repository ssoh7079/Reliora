using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAspect : MonoBehaviour
{
    [SerializeField] private float targetAspect = 16.0f / 9.0f;

    private Camera cam;
    private float baseSize;


    private void Awake()
    {
        cam = GetComponent<Camera>();
        baseSize = cam.orthographicSize;
    }
    void Update()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        if (currentAspect > targetAspect)
        {
            cam.orthographicSize = baseSize * targetAspect / currentAspect;
        }
        else
        {
            cam.orthographicSize = baseSize;
        }
    }
}
