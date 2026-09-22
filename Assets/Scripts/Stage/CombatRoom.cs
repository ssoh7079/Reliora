using UnityEngine;

public class CombatRoom : MonoBehaviour
{
    [Header("래퍼런스")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private RoomBackground roomBackground;

    [Header("카메라 경계")]
    [SerializeField] private BoxCollider2D leftWall;
    [SerializeField] private BoxCollider2D rightWall;

    private bool isClear;

    public Transform PlayerSpawnPoint => playerSpawnPoint;
    public bool IsClear => isClear;

    public BoxCollider2D LeftWall => leftWall;
    public BoxCollider2D RightWall => rightWall;


    private void Awake()
    {
        enemySpawner.OnAllEnemiesDead += ClearRoom;
    }
    private void OnDestroy()
    {
        if (enemySpawner != null) enemySpawner.OnAllEnemiesDead -= ClearRoom;
    }

    public void EnterRoom()
    {
        isClear = false;
        roomBackground.Activate();
        enemySpawner.SpawnEnemies();
        //확인용
        Debug.Log($"{name} Start");
    }
    public void ExitRoom()
    {
        roomBackground.Deactivate();
    }
    private void ClearRoom()
    {
        if (isClear) return;
        isClear = true;
        //확인용
        Debug.Log($"{name} Clear");

        //후에 할 작업
        //맵 중앙에 아티팩트 생성
    }
}
