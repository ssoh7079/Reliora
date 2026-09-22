using UnityEngine;

public class StageController : MonoBehaviour
{
    [Header("래퍼런스")]
    [SerializeField] private PlayerController player;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("전투방")]
    [SerializeField] private CombatRoom[] battleRooms;

    void Start()
    {
        EnterRandomBattleRoom();
    }

    private void EnterRandomBattleRoom()
    {
        if (battleRooms == null || battleRooms.Length == 0) return;

        int index = Random.Range(0, battleRooms.Length);
        CombatRoom room = battleRooms[index];
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        //현재 방의 카메라 좌우 경계 설정
        cameraFollow.SetBounds(room.LeftWall, room.RightWall);
        playerRb.linearVelocity = Vector2.zero;
        player.transform.position = room.PlayerSpawnPoint.position;
        //방들이 서로 멀리 떨어져 있으므로 즉시 이동
        cameraFollow.SnapToTarget();
        //여기서 Background도 현재 Camera 중앙을 기준으로 고정
        room.EnterRoom();
    }
}
