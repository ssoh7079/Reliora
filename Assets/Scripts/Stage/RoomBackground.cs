using UnityEngine;

public class RoomBackground : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private float followRate = 0.95f;

    private Camera mainCam;

    private Vector3 startBackgroundPos;
    private Vector3 startCameraPos;

    private bool isActive;


    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void LateUpdate()
    {
        if (!isActive || mainCam == null) return;

        Vector3 cameraMove = mainCam.transform.position - startCameraPos;

        background.position = new Vector3(
            startBackgroundPos.x + cameraMove.x * followRate, 
            startBackgroundPos.y + cameraMove.y * followRate, 
            startBackgroundPos.z);
    }

    public void Activate()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null || background == null) return;

        Vector3 cameraPos = mainCam.transform.position;
        //방에 들어오는 순간 배경을 카메라 정중앙에 맞춤
        background.position = new Vector3(cameraPos.x, cameraPos.y, background.position.z);

        startBackgroundPos = background.position;
        startCameraPos = cameraPos;

        isActive = true;
    }
    public void Deactivate()
    {
        isActive = false;
    }
}
