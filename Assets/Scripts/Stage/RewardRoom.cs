using UnityEngine;

public class RewardRoom : MonoBehaviour
{
    [Header("래퍼런스")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private RoomBackground roomBackground;
    [SerializeField] private GameObject artifactChest;
    [SerializeField] private GameObject exitPoint;

    [Header("카메라 경계")]
    [SerializeField] private BoxCollider2D leftWall;
    [SerializeField] private BoxCollider2D rightWall;

    public Transform PlayerSpawnPoint => playerSpawnPoint;
    public BoxCollider2D LeftWall => leftWall;
    public BoxCollider2D RightWall => rightWall;


    public void EnterRoom()
    {
        artifactChest.SetActive(true);
        exitPoint.SetActive(false);
        roomBackground.Activate();
        //확인용
        Debug.Log("보상 방 진입");
    }
    public void ExitRoom()
    {
        roomBackground.Deactivate();
    }
    public void RewardTaken()
    {
        exitPoint.SetActive(true);
        //확인용
        Debug.Log("보상 방 아티팩트 획득");
    }
}
