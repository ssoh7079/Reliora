using UnityEngine;

public class CombatRoom : MonoBehaviour
{
    [Header("래퍼런스")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private RoomBackground roomBackground;

    [Header("보상")]
    [SerializeField] private Transform rewardSpawnPoint;
    [SerializeField] private GameObject artifactChestPrefab;

    [Header("출구")]
    [SerializeField] private GameObject exitPoint;

    [Header("카메라 경계")]
    [SerializeField] private BoxCollider2D leftWall;
    [SerializeField] private BoxCollider2D rightWall;

    private bool isClear;
    private GameObject currentChest;

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
        exitPoint.SetActive(false);
        if (currentChest != null)
        {
            Destroy(currentChest);
            currentChest = null;
        }
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
        SpawnReward();
        //확인용
        Debug.Log($"{name} Clear");
    }
    private void SpawnReward()
    {
        currentChest = Instantiate(artifactChestPrefab, rewardSpawnPoint.position, Quaternion.identity, transform);
        ArtifactChest chest = currentChest.GetComponent<ArtifactChest>();
        chest.SetRoom(this);
    }
    public void RewardTaken()
    {
        if (!isClear) return;
        currentChest = null;
        exitPoint.SetActive(true);
        //확인용
        Debug.Log($"{name} 보상 획득");
    }
}
