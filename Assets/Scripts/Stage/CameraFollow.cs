using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("카메라 추적")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = new Vector2(0.0f, 1.0f);
    [SerializeField] private float smoothTime = 0.15f;

    private Camera cam;
    private Vector3 velocity;

    private float minX;
    private float maxX;
    private bool useBounds;


    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = GetTargetPosition();

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    public void SetBounds(BoxCollider2D leftWall, BoxCollider2D rightWall)
    {
        useBounds = false;

        if (cam == null || leftWall == null || rightWall == null) return;

        float leftX = leftWall.bounds.max.x;
        float rightX = rightWall.bounds.min.x;
        if (leftX >= rightX)
        {
            //확인용
            Debug.LogWarning("Camera Bounds 위치를 확인해주세요.");
            return;
        }

        float halfWidth = cam.orthographicSize * cam.aspect;
        minX = leftX + halfWidth;
        maxX = rightX - halfWidth;
        //방 크기가 카메라 화면보다 작은 경우 중앙 고정
        if (minX > maxX)
        {
            float centerX = (leftX + rightX) * 0.5f;
            minX = centerX;
            maxX = centerX;
        }

        useBounds = true;
    }
    public void SnapToTarget()
    {
        if (target == null) return;

        transform.position = GetTargetPosition();
        velocity = Vector3.zero;
    }
    private Vector3 GetTargetPosition()
    {
        float x = target.position.x + offset.x;
        float y = target.position.y + offset.y;

        if (useBounds) x = Mathf.Clamp(x, minX, maxX);

        return new Vector3(x, y, transform.position.z);
    }
}
