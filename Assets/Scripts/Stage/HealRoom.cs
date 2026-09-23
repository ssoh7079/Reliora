using UnityEngine;

public class HealRoom : MonoBehaviour
{
    [Header("래퍼런스")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private RoomBackground roomBackground;
    [SerializeField] private GameObject exitPoint;

    [Header("카메라 경계")]
    [SerializeField] private BoxCollider2D leftWall;
    [SerializeField] private BoxCollider2D rightWall;

    public Transform PlayerSpawnPoint => playerSpawnPoint;
    public BoxCollider2D LeftWall => leftWall;
    public BoxCollider2D RightWall => rightWall;


    public void EnterRoom()
    {
        exitPoint.SetActive(false);
        roomBackground.Activate();
        //확인용
        Debug.Log("HealRoom 시작");
    }
    public void ExitRoom()
    {
        roomBackground.Deactivate();
    }
    public void HealComplete()
    {
        exitPoint.SetActive(true);
        //확인용
        Debug.Log("HealRoom 회복 완료");
    }
}
