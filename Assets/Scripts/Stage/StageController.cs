using UnityEngine;

public class StageController : MonoBehaviour
{
    [Header("래퍼런스")]
    [SerializeField] private PlayerController player;
    [SerializeField] private CameraFollow cameraFollow;

    [Header("전투방")]
    [SerializeField] private CombatRoom[] battleRooms;

    [Header("특수방")]
    [SerializeField] private RewardRoom rewardRoom;

    [Header("UI")]
    [SerializeField] private ResultUI resultUI;

    private CombatRoom currentBattleRoom;
    private RewardRoom currentRewardRoom;

    private CombatRoom lastBattleRoom;

    private PlayerStatus playerStatus;
    private bool isResult;

    private int stageStep;


    private void Awake()
    {
        playerStatus = player.GetComponent<PlayerStatus>();
        playerStatus.OnDead += PlayerDead;
    }
    void Start()
    {
        stageStep = 0;
        EnterBattleRoom();
    }
    private void OnDestroy()
    {
        if (playerStatus != null) playerStatus.OnDead -= PlayerDead;
    }

    public void EnterNextRoom()
    {
        if (isResult) return;
        stageStep++;

        switch (stageStep)
        {
            case 1:
                EnterBattleRoom();
                break;
            case 2:
                EnterRewardRoom();
                break;
            case 3:
                EnterBattleRoom();
                break;
            case 4:
                EnterBattleRoom();
                break;
            case 5:
                //확인용
                //Debug.Log("Battle 4회 완료 - 다음은 HealRoom");
                StageClear();
                break;
        }
    }
    private void EnterBattleRoom()
    {
        ExitCurrentRoom();

        if (battleRooms == null || battleRooms.Length == 0) return;

        CombatRoom room = battleRooms[Random.Range(0, battleRooms.Length)];
        if (battleRooms.Length > 1)
        {
            while (room == lastBattleRoom) room = battleRooms[Random.Range(0, battleRooms.Length)];
        }

        lastBattleRoom = room;
        currentBattleRoom = room;

        MovePlayer(room.PlayerSpawnPoint, room.LeftWall, room.RightWall);
        room.EnterRoom();
    }
    private void EnterRewardRoom()
    {
        ExitCurrentRoom();
        currentRewardRoom = rewardRoom;
        MovePlayer(rewardRoom.PlayerSpawnPoint, rewardRoom.LeftWall, rewardRoom.RightWall);
        rewardRoom.EnterRoom();
    }
    private void MovePlayer(Transform spawnPoint, BoxCollider2D leftWall, BoxCollider2D rightWall)
    {
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        playerRb.linearVelocity = Vector2.zero;
        cameraFollow.SetBounds(leftWall, rightWall);
        player.transform.position = spawnPoint.position;
        cameraFollow.SnapToTarget();
    }
    private void ExitCurrentRoom()
    {
        if (currentBattleRoom != null)
        {
            currentBattleRoom.ExitRoom();
            currentBattleRoom = null;
        }
        if (currentRewardRoom != null)
        {
            currentRewardRoom.ExitRoom();
            currentRewardRoom = null;
        }
    }

    private void StageClear()
    {
        if (isResult) return;

        isResult = true;
        ExitCurrentRoom();
        resultUI.ShowClear();
        //확인용
        Debug.Log("스테이지 클리어");
    }
    private void PlayerDead()
    {
        if (isResult) return;

        isResult = true;
        ExitCurrentRoom();
        resultUI.ShowDead();
    }
}
